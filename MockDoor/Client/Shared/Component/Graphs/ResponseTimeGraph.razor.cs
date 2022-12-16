using Microsoft.AspNetCore.Components;
using MockDoor.Client.Services;
using MockDoor.Client.Shared.Component.Dialog;
using Radzen;

namespace MockDoor.Client.Shared.Component.Graphs
{
    public partial class ResponseTimeGraph
    {
        [Inject]
        public RequestService RequestService { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        [Parameter]
        public int? RequestId { get; set; }

        private const int RangeBuffer = 1000;
        DataItem[] _data;

        bool _showDataLabels;
        int _dateMinInt, _dateMaxInt, _latencyMin, _latencyMax;
        int _currentLatencyMin, _currentLatencyMax = 1;
        DateTime _dateMin, _dateMax;

        IEnumerable<int> _dateRange, _latencyRange;
        DateTime Origin { get; set; } = DateTime.Now;

        protected override async Task OnParametersSetAsync()
        {
            if (RequestId != null)
            {
                var responses = await RequestService.GetRequestAsync(RequestId.Value);

                if (responses.IsSuccessStatusCode && responses.Content != null)
                {

                    _data = responses.Content.MockResponses.Select(mr => new DataItem()
                    {
                        Latency = mr.Latency.Milliseconds,
                        Time = mr.CreatedUtc,
                        MockResponseId = mr.Id
                    }).ToArray();

                    _dateMinInt = ConvertToSecondsTimestamp(_data.First().Time) - RangeBuffer;
                    _dateMaxInt = ConvertToSecondsTimestamp(_data.Last().Time) + RangeBuffer;
                    _dateMin = _data.First().Time; // ConvertToDateTimeTimestamp(dateRange.First()))
                    _dateMax = _data.Last().Time; // ConvertToDateTimeTimestamp(dateRange.Last()))
                    _dateRange = new [] { _dateMinInt, _dateMaxInt };

                    _latencyMin = 0;
                    _currentLatencyMax = _latencyMax = _data.Max(d => d.Latency);

                    _latencyRange = new [] { _latencyMin, _latencyMax };
                    StateHasChanged();
                }
            }
        }

        public int ConvertToSecondsTimestamp(DateTime date)
        {
            TimeSpan diff = date - Origin;
            return Convert.ToInt32(diff.TotalMilliseconds);
        }


        public DateTime ConvertToDateTimeTimestamp(int date)
        {
            return (Origin + TimeSpan.FromMilliseconds(date));
        }

        void UpdateLatencyRange(dynamic args)
        {
            var values = args as IEnumerable<int>;

            _latencyRange = values?.ToList();
            if (_latencyRange != null)
            {
                _currentLatencyMin = _latencyRange.Min();
                _currentLatencyMax = _latencyRange.Max();
                StateHasChanged();
            }
        }

        void UpdateDateRange(dynamic args)
        {
            var values = args as IEnumerable<int>;

            _dateRange = values?.ToList();

            if (_dateRange != null)
            {
                _dateMin = ConvertToDateTimeTimestamp(_dateRange.Min());
                _dateMax = ConvertToDateTimeTimestamp(_dateRange.Max());
                StateHasChanged();
            }
        }

        void SeriesClick(SeriesClickEventArgs args)
        {
            DataItem clickedItem = args.Data as DataItem;

            if (clickedItem != null)
            {
                DialogService.Open<GraphRequestClickedDialog>($"View response Details \"{clickedItem.MockResponseId}\"",
                    new Dictionary<string, object>() { { "ResponseId", clickedItem.MockResponseId } },
                    new DialogOptions()
                    {
                        ShowClose = true, ShowTitle = true, Width = "1480px", Height = "1320px", Resizable = true,
                        Draggable = true
                    });
            }
        }

        readonly TimeSpan _step = new(0, 1, 5);

        class DataItem
        {
            public DateTime Time { get; set; }
            public int Latency { get; set; }
            public int MockResponseId { get; set; }
        }

    }
}
