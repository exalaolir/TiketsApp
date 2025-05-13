using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public int RootCategoryId { get; set; }

        public Category? RootCategory { get; set; }

        public int SubCategoryId { get; set; }

        public Category? SubCategory { get; set; }

        public List<Image> Emages { get; set; } = new();

        public int Count { get; set; }

        public required int MaxCount { get; set; }

        public string? SeatMap { get; set; }

        public required DateTime StartTime { get; set; }

        public required DateTime EndTime { get; set; }

        public required string Adress { get; set; }

        public required decimal Price { get; set; }

        public int SallerId { get; set; }
        public Saller? Saller { get; set; }
    }

    [JsonConverter(typeof(SeatMapJsonConverter))]
    public sealed class SeatMap
    {
        private readonly List<Row> _rows;

        public int RowsCount => _rows.Count;
        public int SeatsCount => _rows.Count > 0 ? _rows[0].SeatsCount : 0;

        public class Row
        {
            private readonly List<Seat> _seats;

            public int Number { get; }
            public int SeatsCount => _seats.Count;

            public Seat this[int index]
            {
                get => _seats[index - 1];
                set => _seats[index - 1] = value;
            }

            public Row ( int seats, int number )
            {
                Number = number + 1;
                _seats = new List<Seat>(seats);
                for (int i = 0; i < seats; i++)
                {
                    _seats.Add(new Seat(i));
                }
            }
        }

        public class Seat
        {
            public int Number { get; }
            public bool IsOwned { get; set; }

            public Seat ( int number )
            {
                Number = number;
            }
        }

        public Row this[int index] => _rows[index - 1];

        public SeatMap ( int rows, int seats )
        {
            _rows = new List<Row>(rows);
            for (int i = 0; i < rows; i++)
            {
                _rows.Add(new Row(seats, i));
            }
        }
    }
}
