using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleWorldMapPlotter
{
    public class PlaneDataProvider
    {
        private const string Name = "Name";
        private const string BaroAltitude = "Baro Altitude";
        private const string CallSign = "CallSign";
        private const string GeoAltitude = "Geo Altitude";
        private const string GroundSpeed = "Ground Speed";
        private const string ICAO24 = "ICAO24";
        private const string IsOnGround = "Is On Ground";
        private const string IsSpecialPurpose = "Is Special Purpose";
        private const string LastPositionTime = "Last Position Time";
        private const string LastUpdateTime = "Last Update Time";
        private const string Latitude = "Latitude";
        private const string Longitude = "Longitude";
        private const string PositionSource = "Position Source";
        private const string Squawk = "Squawk";
        private const string TrueTrack = "True Track";
        private const string VerticalSpeed = "Vertical Speed";

        private ConcurrentDictionary<string, AFElement> _callSignToElementMap
           = new ConcurrentDictionary<string, AFElement>();

        private List<string> _callSignList = new List<string>();
       
        private PISystem _piSystem = null;
        private AFDatabase _db = null;
        private AFElement _planeElement = null;

        public PlaneDataProvider()
        {
            PISystems piSession = new PISystems(false);
            _piSystem = piSession["Blobfishgsvm"];
            _db = _piSystem.Databases["ADSB"];
            _planeElement = _db.Elements["Planes"];

            GetCallSignNames(true);
        }

        public bool Initialized { get; private set; } = false;

        public async Task StartAsync(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                try
                {
                    foreach (var planeElement in _planeElement.Elements)
                    {
                        var attributes = planeElement.Attributes;
                        var callSign = TryGetAttributeValue(planeElement, CallSign);

                        if (!string.IsNullOrEmpty(callSign))
                        {
                            _callSignToElementMap.AddOrUpdate(
                                callSign, 
                                planeElement, 
                                (k, old) => planeElement);
                        }
                    }

                    Initialized = true;
                }
                catch(Exception ex)
                {
                   Initialized = false;
                }
                finally
                {
                    await Task.Delay(3600000);
                }
            }           
        }

        public IList<string> GetCallSignNames(bool update = false)
        {
            if (update)
            {
                var callSignDic = _planeElement
               .Elements
               .Select(e => e.Attributes[CallSign].GetValue().Value.ToString())
               .ToList();

                Interlocked.Exchange(ref _callSignList, callSignDic);
            }
           
            return _callSignList;
        }

        public IDictionary<string, string> GetLatestPlaneData(string callSign)
        {
            if (!_callSignToElementMap.ContainsKey(callSign))
            {
                return new Dictionary<string, string>();
            }

            var planeElement = _callSignToElementMap[callSign];           
            var planeDataObj = new Dictionary<string, string>()
            {
                {  nameof(Name), planeElement.Name },
                {  nameof(BaroAltitude), TryGetAttributeValue(planeElement, BaroAltitude) },
                {  nameof(GeoAltitude), TryGetAttributeValue(planeElement, GeoAltitude) },
                {  nameof(CallSign), callSign },
                {  nameof(GroundSpeed), TryGetAttributeValue(planeElement, GroundSpeed) },
                {  nameof(ICAO24), TryGetAttributeValue(planeElement, ICAO24)  },
                {  nameof(IsOnGround), TryGetAttributeValue(planeElement, IsOnGround) },
                {  nameof(IsSpecialPurpose), TryGetAttributeValue(planeElement, IsSpecialPurpose) },
                {  nameof(LastPositionTime), TryGetAttributeValue(planeElement, LastPositionTime) },
                {  nameof(LastUpdateTime), TryGetAttributeValue(planeElement, LastUpdateTime) },
                {  nameof(Latitude), TryGetAttributeValue(planeElement, Latitude)},
                {  nameof(Longitude), TryGetAttributeValue(planeElement, Longitude) },
                {  nameof(PositionSource), TryGetAttributeValue(planeElement, PositionSource)  },
                {  nameof(Squawk), TryGetAttributeValue(planeElement, Squawk) },
                {  nameof(TrueTrack), TryGetAttributeValue(planeElement, TrueTrack) },
                {  nameof(VerticalSpeed), TryGetAttributeValue(planeElement, VerticalSpeed) }
            };

            return planeDataObj;
        }

        public IDictionary<string, string> GetPlaneDataAtTime(string callSign, string time)
        {
            if (!_callSignToElementMap.ContainsKey(callSign))
            {
                return new Dictionary<string, string>();
            }

            var afTime = new AFTime(time);
            var planeElement = _callSignToElementMap[callSign];
            var planeDataObj = new Dictionary<string, string>()
            {
                {  nameof(Name), planeElement.Name },
                {  nameof(BaroAltitude), TryGetAttributeValue(planeElement, BaroAltitude, afTime) },
                {  nameof(GeoAltitude), TryGetAttributeValue(planeElement, GeoAltitude, afTime) },
                {  nameof(CallSign), callSign },
                {  nameof(GroundSpeed), TryGetAttributeValue(planeElement, GroundSpeed, afTime) },
                {  nameof(ICAO24), TryGetAttributeValue(planeElement, ICAO24)},
                {  nameof(IsOnGround), TryGetAttributeValue(planeElement, IsOnGround, afTime) },
                {  nameof(IsSpecialPurpose), TryGetAttributeValue(planeElement, IsSpecialPurpose, afTime)},
                {  nameof(LastPositionTime), TryGetAttributeValue(planeElement, LastPositionTime, afTime) },
                {  nameof(LastUpdateTime), TryGetAttributeValue(planeElement, LastUpdateTime, afTime) },
                {  nameof(Latitude), TryGetAttributeValue(planeElement, Latitude, afTime)},
                {  nameof(Longitude), TryGetAttributeValue(planeElement, Longitude, afTime) },
                {  nameof(PositionSource), TryGetAttributeValue(planeElement, PositionSource, afTime)},
                {  nameof(Squawk), TryGetAttributeValue(planeElement, Squawk, afTime)},
                {  nameof(TrueTrack), TryGetAttributeValue(planeElement, TrueTrack, afTime) },
                {  nameof(VerticalSpeed), TryGetAttributeValue(planeElement, VerticalSpeed, afTime) }
            };

            return planeDataObj;
        }
              
        public IDictionary<DateTime, Dictionary<string, string>> GetPlaneDataInInterval(string callSign, string fromTime, string toTime)
        {
            var dateTimeToPlaneDataMap = new SortedDictionary<DateTime, Dictionary<string, string>>();

            if (_callSignToElementMap.ContainsKey(callSign))
            {
                var planeElement = _callSignToElementMap[callSign];

                AFTime startTime = new AFTime(fromTime);
                AFTime endTime = new AFTime(toTime);

                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(BaroAltitude), BaroAltitude, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(GeoAltitude), GeoAltitude, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(GroundSpeed), GroundSpeed, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(ICAO24), ICAO24, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(IsOnGround), IsOnGround, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(IsSpecialPurpose), IsSpecialPurpose, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(LastPositionTime), LastPositionTime, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(LastUpdateTime), LastUpdateTime, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(Latitude), Latitude, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(Longitude), Longitude, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(PositionSource), PositionSource, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(Squawk), Squawk, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(TrueTrack), TrueTrack, startTime, endTime);
                GetAttrValuesAndStoreToMap(dateTimeToPlaneDataMap, planeElement, nameof(VerticalSpeed), VerticalSpeed, startTime, endTime);
            }

            return dateTimeToPlaneDataMap;
        }

        private static void GetAttrValuesAndStoreToMap(
            SortedDictionary<DateTime, Dictionary<string, string>> dateTimeToPlaneDataMap, 
            AFElement planeElement,             
            string attrNameKey,
            string attrName,
            AFTime startTime, 
            AFTime endTime)
        {
            var timeToDataList = TryGetAttributeValues(planeElement, attrName, startTime, endTime);
            Console.WriteLine($"Attr: {attrNameKey}, GetValues Count: {timeToDataList.Count}");

            foreach (var item in timeToDataList)
            {
                if (dateTimeToPlaneDataMap.ContainsKey(item.Item1))
                {
                    var dic = dateTimeToPlaneDataMap[item.Item1];
                    if (dic == null)
                    {
                        dic = new Dictionary<string, string>() { { attrNameKey, item.Item2.ToString() } };
                    }
                    else
                    {
                        if (dic.ContainsKey(attrNameKey))
                        {
                            Console.WriteLine($"Key already exists: {attrNameKey}");
                        }

                        dic[attrNameKey] = item.Item2.ToString();
                    }
                }
                else
                {
                    dateTimeToPlaneDataMap.Add(
                        item.Item1,
                        new Dictionary<string, string>() { { attrNameKey, item.Item2.ToString() } });
                }
            }
        }

        public static string TryGetAttributeValue(
          AFElement element,
          string attrName)
        {
            try
            {
                return element.Attributes[attrName].GetValue().Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get a value for {attrName}: {ex}");
            }

            return string.Empty;
        }

        public static string TryGetAttributeValue(
            AFElement element, 
            string attrName, 
            AFTime time)
        {
            try
            {
                return element.Attributes[attrName].GetValue(time).Value.ToString();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to get a value for {attrName}: {ex}");
            }

            return string.Empty;
        }

        public static List<Tuple<DateTime, string>> TryGetAttributeValues(
             AFElement element,
             string attrName,
             AFTime startTime,
             AFTime endTime)
        {
            List<Tuple<DateTime, string>> ret = new List<Tuple<DateTime, string>>();
            try
            {
                var range = new AFTimeRange(startTime, endTime);
                var timeSpan = new AFTimeSpan(TimeSpan.FromMinutes(30));
                var values = element
                    .Attributes[attrName]
                    .Data.InterpolatedValues(range, timeSpan, null, null, false);

                return values.Select(i => Tuple.Create(i.Timestamp.UtcTime, i.Value.ToString())).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get a value for {attrName}: {ex}");
            }

            return ret;
        }
    }
}
