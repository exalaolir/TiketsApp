using System.Text.Json;
using System.Text.Json.Serialization;
using TiketsApp.Models;

public class SeatMapJsonConverter : JsonConverter<SeatMap>
{
    public override SeatMap Read ( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        var seatMapData = JsonSerializer.Deserialize<SeatMapData>(ref reader, options);

        if (seatMapData == null) throw new ArgumentNullException(nameof(seatMapData));  

        var seatMap = new SeatMap(seatMapData.Rows.Count, seatMapData.Rows[0].Seats.Count);

        for (int i = 0; i < seatMapData.Rows.Count; i++)
        {
            var rowData = seatMapData.Rows[i];
            var row = seatMap[i + 1]; 

            for (int j = 0; j < rowData.Seats.Count; j++)
            {
                var seatData = rowData.Seats[j];
                var seat = row[j + 1]; 
                seat.IsOwned = seatData.IsOwned;
            }
        }

        return seatMap;
    }

    public override void Write ( Utf8JsonWriter writer, SeatMap value, JsonSerializerOptions options )
    {
        var seatMapData = new SeatMapData
        {
            Rows = new List<RowData>(value.RowsCount)
        };

        for (int i = 1; i <= value.RowsCount; i++)
        {
            var row = value[i];
            var rowData = new RowData
            {
                Number = row.Number,
                Seats = new List<SeatData>(row.SeatsCount)
            };

            for (int j = 1; j <= row.SeatsCount; j++)
            {
                var seat = row[j];
                rowData.Seats.Add(new SeatData
                {
                    Number = seat.Number,
                    IsOwned = seat.IsOwned
                });
            }

            seatMapData.Rows.Add(rowData);
        }

        JsonSerializer.Serialize(writer, seatMapData, options);
    }


    private class SeatMapData
    {
        public List<RowData> Rows { get; set; } = [];
    }

    private class RowData
    {
        public int Number { get; set; }
        public List<SeatData> Seats { get; set; } = [];
    }

    private class SeatData
    {
        public int Number { get; set; }
        public bool IsOwned { get; set; }
    }
}