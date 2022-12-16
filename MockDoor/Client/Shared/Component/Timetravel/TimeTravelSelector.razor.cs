using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MockDoor.Client.Helpers;
using MockDoor.Client.Models;
using MockDoor.Client.Services;
using MockDoor.Shared;
using MockDoor.Shared.Models.Timetravel;
using Radzen;

namespace MockDoor.Client.Shared.Component.Timetravel
{
    public partial class TimeTravelSelector
    {
        [Inject]
        public SimulateTimeService SimulateTimeService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        [Parameter]
        public TimeTravelScope? Scope { get; set; }

        [Parameter]
        public EventCallback<DateTime?> OnChanged { get; set; }

        [Parameter]
        public bool ShowSubmit { get; set; } = true;

        [Parameter]
        public bool SubmitOnChange { get; set; }

        [Parameter]
        public DateTime? CurrentDateTime { get; set; }

        List<TimeTravelItem> _timeTravelItems = new();

        int _sliderIndex = -1;

        string _errorMessage;

        private int Max => _timeTravelItems?.Count - 1 ?? 1;

        protected override async Task OnParametersSetAsync()
        {
            if (Id != null && Scope != null && Id.Value > 0)
            {
                var response = await SimulateTimeService.GetSimulateTimes(Scope.Value, Id.Value);

                if (response.IsSuccessStatusCode)
                {
                    var timeTravelDto = response.Content;
                    CreateTimeTravelList(timeTravelDto);

                    StateHasChanged();
                }
            }
        }
        
        #region Slider
        private void CreateTimeTravelList(TimeTravelDto timeTravelDto)
        {
            var availableTimes = timeTravelDto.AvailableTimes;

            // Add the current set time if not in list to list
            if (timeTravelDto.CurrentTime != null && !availableTimes.Any(t => t == timeTravelDto.CurrentTime))
            {
                availableTimes.Add(timeTravelDto.CurrentTime.Value);
            }

            // Add all available times to list
            _timeTravelItems.Clear();
            foreach (var availableTime in availableTimes)
            {
                bool currentValue = availableTime == timeTravelDto.CurrentTime;
                
                _timeTravelItems.Add(new TimeTravelItem()
                {
                    Time = availableTime,
                    Description = currentValue ? $"{availableTime} (Initial)" : availableTime.ToString(CultureInfo.CurrentCulture)
                });
            }
            _timeTravelItems = _timeTravelItems.OrderBy(t => t.Time).ToList();
            
            _timeTravelItems.Add(new TimeTravelItem()
            {
                Time = null,
                Description = "Live/Latest"
            });

            if (_timeTravelItems.Any(tt => tt.Time == CurrentDateTime))
            {
                _sliderIndex = _timeTravelItems.IndexOf(_timeTravelItems.First(tt => tt.Time == CurrentDateTime));
            }
            else if(_timeTravelItems.Any(tt => tt.Time >= CurrentDateTime))
            {
                _sliderIndex = _timeTravelItems.IndexOf(_timeTravelItems.First(tt => tt.Time >= CurrentDateTime));
            }
            else
            {
                _sliderIndex = 0;
            }
        }

        async Task OnSetSliderAsync(int value)
        {
            await UpdateCurrentDateTimeAsync(_timeTravelItems[value].Time);
            if (value == Max)
            {
                _sliderIndex = Max;
            }
            else
            {
                _sliderIndex = _timeTravelItems.IndexOf(_timeTravelItems.First(tt => tt.Time == CurrentDateTime));
            }
        }

        async Task FirstAsync()
        {            
            await OnSetSliderAsync(0);
        }

        async Task PreviousAsync()
        {
            if (_sliderIndex > 0)
            {
                await OnSetSliderAsync(_sliderIndex - 1);
            }
        }

        async Task NextAsync()
        {
            if (_sliderIndex < Max)
            {
                await OnSetSliderAsync(_sliderIndex + 1);
            }
        }

        async Task LastAsync()
        {
            await OnSetSliderAsync(Max);
        }
        #endregion

        void DateRenderSpecial(DateRenderEventArgs args)
        {
            if (_timeTravelItems?.Where(t => t.Time.HasValue).Select(t => t.Time.Value.Date).Contains(args.Date) ?? false)
            {
                args.Attributes.Add("style", "background-color: #ff6d41; border-color: white;");
            }
        }

        bool HasMinDateTimeLoaded()
        {

            if (_timeTravelItems == null)
            {
                return false;
            }
            return true;
        }

        DateTime? GetMinDateTimeIfExists()
        {
            if (_timeTravelItems == null || _timeTravelItems.Count == 0)
            {
                return null;
            }
            var datetime = _timeTravelItems[0]?.Time;

            if (datetime == null)
            {
                return null;
            }

            return datetime.Value.Date;
        }

        DateTime GetMinDate()
        {
            var datetime = GetMinDateTimeIfExists();

            if(datetime == null)
            {
                return DateTime.MinValue;
            }

            return datetime.Value.Date;
        }

        async Task UpdateCurrentDateTimeAsync(DateTime? value = null)
        {
            if (CurrentDateTime != value)
            {
                CurrentDateTime = value;
                await OnChanged.InvokeAsync(CurrentDateTime);
            }

            if (SubmitOnChange)
            {
                await SubmitAsync();
            }
        }

        async Task SubmitAsync()
        {
           _errorMessage = string.Empty;
            try
            {
                if (Id != null && Scope != null)
                {
                    var response = await SimulateTimeService.SetSimulateTime(Id.Value, CurrentDateTime, Scope.Value);

                    if (!response.IsSuccessStatusCode)
                    {
                        _errorMessage = response.Message;
                    }
                }
            }
            catch(Exception ex)
            {
                _errorMessage = ex.Message;
            }
        }
    }
}
