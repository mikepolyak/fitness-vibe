using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.HealthData;
using FitnessVibe.Application.Queries.HealthData;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Health Data Controller - the bridge connecting your fitness journey with the entire health ecosystem.
    /// Think of this as the universal translator that speaks with your smartwatch, fitness tracker,
    /// phone's health app, and other wellness devices. It's like having a personal data assistant
    /// that gathers all your health information from every source and presents it in one unified view,
    /// while respecting your privacy and giving you complete control over your data.
    /// </summary>
    [ApiController]
    [Route("api/health-data")]
    [Authorize]
    [Produces("application/json")]
    public class HealthDataController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HealthDataController> _logger;

        public HealthDataController(IMediator mediator, ILogger<HealthDataController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get connected data sources and their sync status.
        /// Like checking which of your health devices and apps are talking to your fitness hub.
        /// </summary>
        /// <returns>Connected health data sources</returns>
        /// <response code="200">Data sources retrieved successfully</response>
        [HttpGet("sources")]
        [ProducesResponseType(typeof(ConnectedSourcesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ConnectedSourcesResponse>> GetConnectedSources()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetConnectedSourcesQuery { UserId = userId };

                _logger.LogDebug("Getting connected health data sources for user: {UserId}", userId);

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get connected sources for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Connected Sources",
                    Detail = "Unable to retrieve your connected health data sources.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Connect a new health data source (HealthKit, Google Fit, Fitbit, etc.).
        /// Like pairing a new fitness device with your central health dashboard.
        /// </summary>
        /// <param name="command">Data source connection details</param>
        /// <returns>Connection confirmation and setup instructions</returns>
        /// <response code="201">Data source connected successfully</response>
        /// <response code="400">Invalid connection data</response>
        /// <response code="409">Data source already connected</response>
        [HttpPost("sources/connect")]
        [ProducesResponseType(typeof(ConnectSourceResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ConnectSourceResponse>> ConnectDataSource([FromBody] ConnectDataSourceCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Connecting health data source for user {UserId}: {SourceType}", 
                    userId, command.SourceType);

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetConnectedSources),
                    new { },
                    result
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already connected"))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Data Source Already Connected",
                    Detail = "This health data source is already connected to your account.",
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect data source for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Data Source Connection Failed",
                    Detail = "Unable to connect the health data source. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Disconnect a health data source.
        /// Like unplugging a device from your health dashboard.
        /// </summary>
        /// <param name="sourceId">Data source ID to disconnect</param>
        /// <returns>Disconnection confirmation</returns>
        /// <response code="200">Data source disconnected successfully</response>
        /// <response code="404">Data source not found</response>
        [HttpPost("sources/{sourceId}/disconnect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DisconnectDataSource(int sourceId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new DisconnectDataSourceCommand 
                { 
                    UserId = userId,
                    SourceId = sourceId
                };

                await _mediator.Send(command);

                _logger.LogInformation("Disconnected data source {SourceId} for user: {UserId}", sourceId, userId);

                return Ok(new { message = "Data source disconnected successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Data Source Not Found",
                    Detail = "The specified health data source could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disconnect data source {SourceId} for user: {UserId}", sourceId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Data Source Disconnection Failed",
                    Detail = "Unable to disconnect the health data source.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Manually trigger data synchronization from connected sources.
        /// Like pressing a refresh button to get the latest data from all your health devices.
        /// </summary>
        /// <param name="sourceIds">Optional: specific source IDs to sync (empty = sync all)</param>
        /// <returns>Synchronization status</returns>
        /// <response code="200">Synchronization initiated successfully</response>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(SyncHealthDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SyncHealthDataResponse>> SyncHealthData([FromBody] SyncHealthDataCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Initiating health data sync for user: {UserId}", userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync health data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Health Data Sync Failed",
                    Detail = "Unable to synchronize health data from connected sources.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get consolidated health metrics from all sources.
        /// Like viewing your complete health dashboard with data from all connected devices.
        /// </summary>
        /// <param name="startDate">Start date for metrics (optional)</param>
        /// <param name="endDate">End date for metrics (optional, defaults to today)</param>
        /// <param name="granularity">Data granularity (hour, day, week, month)</param>
        /// <returns>Consolidated health metrics</returns>
        /// <response code="200">Health metrics retrieved successfully</response>
        [HttpGet("metrics")]
        [ProducesResponseType(typeof(HealthMetricsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<HealthMetricsResponse>> GetHealthMetrics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string granularity = "day")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetHealthMetricsQuery 
                { 
                    UserId = userId,
                    StartDate = startDate ?? DateTime.Today.AddDays(-30),
                    EndDate = endDate ?? DateTime.Today,
                    Granularity = granularity
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get health metrics for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Health Metrics",
                    Detail = "Unable to retrieve your health metrics.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Submit manual health data entry.
        /// Like logging health information when your devices aren't capturing everything.
        /// </summary>
        /// <param name="command">Manual health data entry</param>
        /// <returns>Data entry confirmation</returns>
        /// <response code="201">Health data logged successfully</response>
        /// <response code="400">Invalid health data</response>
        [HttpPost("manual-entry")]
        [ProducesResponseType(typeof(ManualHealthDataResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ManualHealthDataResponse>> LogManualHealthData([FromBody] LogManualHealthDataCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Logging manual health data for user {UserId}: {DataType}", 
                    userId, command.DataType);

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetHealthMetrics),
                    new { },
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log manual health data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Health Data Logging Failed",
                    Detail = "Unable to log your health data entry. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get detailed heart rate data from connected devices.
        /// Like viewing your heart rate monitor's complete history.
        /// </summary>
        /// <param name="startDate">Start date for heart rate data</param>
        /// <param name="endDate">End date for heart rate data</param>
        /// <param name="dataType">Heart rate data type (resting, active, max, zones)</param>
        /// <returns>Heart rate data</returns>
        /// <response code="200">Heart rate data retrieved successfully</response>
        [HttpGet("heart-rate")]
        [ProducesResponseType(typeof(HeartRateDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<HeartRateDataResponse>> GetHeartRateData(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string dataType = "all")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetHeartRateDataQuery 
                { 
                    UserId = userId,
                    StartDate = startDate ?? DateTime.Today.AddDays(-7),
                    EndDate = endDate ?? DateTime.Today,
                    DataType = dataType
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get heart rate data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Heart Rate Data",
                    Detail = "Unable to retrieve your heart rate data.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get sleep data from connected sources.
        /// Like reviewing your sleep tracker's complete sleep analysis.
        /// </summary>
        /// <param name="startDate">Start date for sleep data</param>
        /// <param name="endDate">End date for sleep data</param>
        /// <returns>Sleep analysis data</returns>
        /// <response code="200">Sleep data retrieved successfully</response>
        [HttpGet("sleep")]
        [ProducesResponseType(typeof(SleepDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SleepDataResponse>> GetSleepData(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetSleepDataQuery 
                { 
                    UserId = userId,
                    StartDate = startDate ?? DateTime.Today.AddDays(-30),
                    EndDate = endDate ?? DateTime.Today
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sleep data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Sleep Data",
                    Detail = "Unable to retrieve your sleep data.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get body composition and weight data.
        /// Like checking your smart scale's complete history.
        /// </summary>
        /// <param name="startDate">Start date for body data</param>
        /// <param name="endDate">End date for body data</param>
        /// <param name="includeComposition">Include body composition data (fat %, muscle mass, etc.)</param>
        /// <returns>Body composition and weight data</returns>
        /// <response code="200">Body data retrieved successfully</response>
        [HttpGet("body-composition")]
        [ProducesResponseType(typeof(BodyCompositionDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<BodyCompositionDataResponse>> GetBodyCompositionData(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool includeComposition = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetBodyCompositionDataQuery 
                { 
                    UserId = userId,
                    StartDate = startDate ?? DateTime.Today.AddDays(-90),
                    EndDate = endDate ?? DateTime.Today,
                    IncludeComposition = includeComposition
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get body composition data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Body Composition Data",
                    Detail = "Unable to retrieve your body composition data.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get nutrition and hydration data from connected apps.
        /// Like reviewing your food diary and water intake tracker.
        /// </summary>
        /// <param name="startDate">Start date for nutrition data</param>
        /// <param name="endDate">End date for nutrition data</param>
        /// <param name="includeHydration">Include water intake data</param>
        /// <returns>Nutrition and hydration data</returns>
        /// <response code="200">Nutrition data retrieved successfully</response>
        [HttpGet("nutrition")]
        [ProducesResponseType(typeof(NutritionDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<NutritionDataResponse>> GetNutritionData(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool includeHydration = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetNutritionDataQuery 
                { 
                    UserId = userId,
                    StartDate = startDate ?? DateTime.Today.AddDays(-7),
                    EndDate = endDate ?? DateTime.Today,
                    IncludeHydration = includeHydration
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get nutrition data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Nutrition Data",
                    Detail = "Unable to retrieve your nutrition data.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Export health data for backup or external analysis.
        /// Like creating a complete backup of all your health information.
        /// </summary>
        /// <param name="format">Export format (json, csv, fhir)</param>
        /// <param name="startDate">Start date for export</param>
        /// <param name="endDate">End date for export</param>
        /// <param name="includeRawData">Include raw sensor data</param>
        /// <returns>Export file information</returns>
        /// <response code="200">Health data export initiated successfully</response>
        [HttpPost("export")]
        [ProducesResponseType(typeof(ExportHealthDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExportHealthDataResponse>> ExportHealthData(
            [FromQuery] string format = "json",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool includeRawData = false)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new ExportHealthDataCommand 
                { 
                    UserId = userId,
                    Format = format,
                    StartDate = startDate,
                    EndDate = endDate,
                    IncludeRawData = includeRawData
                };

                _logger.LogInformation("Exporting health data for user {UserId}, format: {Format}", userId, format);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export health data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Health Data Export Failed",
                    Detail = "Unable to export your health data.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get health data privacy settings and permissions.
        /// Like checking which apps and services have access to your health information.
        /// </summary>
        /// <returns>Health data privacy settings</returns>
        /// <response code="200">Privacy settings retrieved successfully</response>
        [HttpGet("privacy-settings")]
        [ProducesResponseType(typeof(HealthPrivacySettingsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<HealthPrivacySettingsResponse>> GetHealthPrivacySettings()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetHealthPrivacySettingsQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get health privacy settings for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Privacy Settings",
                    Detail = "Unable to retrieve your health data privacy settings.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update health data privacy settings and permissions.
        /// Like adjusting who can see and access your health information.
        /// </summary>
        /// <param name="command">Updated privacy settings</param>
        /// <returns>Updated privacy settings</returns>
        /// <response code="200">Privacy settings updated successfully</response>
        [HttpPut("privacy-settings")]
        [ProducesResponseType(typeof(UpdateHealthPrivacySettingsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UpdateHealthPrivacySettingsResponse>> UpdateHealthPrivacySettings(
            [FromBody] UpdateHealthPrivacySettingsCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update health privacy settings for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Update Privacy Settings",
                    Detail = "Unable to update your health data privacy settings.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get health insights and AI-powered recommendations.
        /// Like having a personal health analyst review all your data and give you insights.
        /// </summary>
        /// <param name="timeframe">Analysis timeframe (week, month, quarter, year)</param>
        /// <param name="includeRecommendations">Include AI-powered health recommendations</param>
        /// <returns>Health insights and recommendations</returns>
        /// <response code="200">Health insights retrieved successfully</response>
        [HttpGet("insights")]
        [ProducesResponseType(typeof(HealthInsightsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<HealthInsightsResponse>> GetHealthInsights(
            [FromQuery] string timeframe = "month",
            [FromQuery] bool includeRecommendations = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetHealthInsightsQuery 
                { 
                    UserId = userId,
                    Timeframe = timeframe,
                    IncludeRecommendations = includeRecommendations
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get health insights for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Health Insights",
                    Detail = "Unable to retrieve your health insights.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get current user ID from JWT claims.
        /// Like reading the member ID from the gym access card.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }

    // Request/Response DTOs for Health Data

    public class ConnectedSourcesResponse
    {
        public List<ConnectedSourceResponse> Sources { get; set; } = new();
        public int TotalSources { get; set; }
        public int ActiveSources { get; set; }
        public DateTime LastSyncTime { get; set; }
        public HealthDataSummaryResponse Summary { get; set; } = new();
    }

    public class ConnectedSourceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // HealthKit, GoogleFit, Fitbit, Garmin, etc.
        public string Status { get; set; } = string.Empty; // Connected, Syncing, Error, Disconnected
        public DateTime ConnectedAt { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public List<string> DataTypes { get; set; } = new();
        public Dictionary<string, string> Permissions { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool IsActive { get; set; }
        public string? DeviceModel { get; set; }
        public string? AppVersion { get; set; }
    }

    public class HealthDataSummaryResponse
    {
        public int TotalDataPoints { get; set; }
        public DateTime OldestDataPoint { get; set; }
        public DateTime NewestDataPoint { get; set; }
        public Dictionary<string, int> DataTypesCounts { get; set; } = new();
        public List<string> MissingDataTypes { get; set; } = new();
        public double DataCompletenessScore { get; set; }
    }

    public class ConnectSourceResponse
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ConnectedAt { get; set; }
        public string? AuthorizationUrl { get; set; }
        public List<string> RequestedPermissions { get; set; } = new();
        public string? SetupInstructions { get; set; }
        public bool RequiresUserAction { get; set; }
    }

    public class SyncHealthDataResponse
    {
        public DateTime SyncStartedAt { get; set; }
        public List<SourceSyncStatusResponse> SourceStatuses { get; set; } = new();
        public int TotalSources { get; set; }
        public int SuccessfulSources { get; set; }
        public int FailedSources { get; set; }
        public string OverallStatus { get; set; } = string.Empty;
        public TimeSpan EstimatedDuration { get; set; }
    }

    public class SourceSyncStatusResponse
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastSyncTime { get; set; }
        public int NewDataPoints { get; set; }
        public string? ErrorMessage { get; set; }
        public TimeSpan SyncDuration { get; set; }
    }

    public class HealthMetricsResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Granularity { get; set; } = string.Empty;
        public ActivityMetricsResponse Activity { get; set; } = new();
        public VitalSignsResponse VitalSigns { get; set; } = new();
        public BodyMetricsResponse Body { get; set; } = new();
        public SleepMetricsResponse Sleep { get; set; } = new();
        public NutritionMetricsResponse Nutrition { get; set; } = new();
        public List<HealthTrendResponse> Trends { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class ActivityMetricsResponse
    {
        public List<DailyActivityResponse> DailyActivity { get; set; } = new();
        public int AverageSteps { get; set; }
        public double AverageDistance { get; set; }
        public int AverageCalories { get; set; }
        public TimeSpan AverageActiveTime { get; set; }
        public int ActiveDays { get; set; }
        public double ConsistencyScore { get; set; }
    }

    public class DailyActivityResponse
    {
        public DateTime Date { get; set; }
        public int Steps { get; set; }
        public double Distance { get; set; }
        public int CaloriesBurned { get; set; }
        public TimeSpan ActiveTime { get; set; }
        public int FlightsClimbed { get; set; }
        public List<ActivityZoneResponse> ActivityZones { get; set; } = new();
    }

    public class ActivityZoneResponse
    {
        public string Zone { get; set; } = string.Empty; // Sedentary, Light, Moderate, Vigorous
        public TimeSpan Duration { get; set; }
        public int CaloriesBurned { get; set; }
    }

    public class VitalSignsResponse
    {
        public List<HeartRateDataPointResponse> HeartRate { get; set; } = new();
        public List<BloodPressureDataPointResponse> BloodPressure { get; set; } = new();
        public List<TemperatureDataPointResponse> BodyTemperature { get; set; } = new();
        public List<OxygenSaturationDataPointResponse> OxygenSaturation { get; set; } = new();
        public VitalSignsSummaryResponse Summary { get; set; } = new();
    }

    public class HeartRateDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public int BeatsPerMinute { get; set; }
        public string Context { get; set; } = string.Empty; // Resting, Active, Workout, Sleep
        public string? Source { get; set; }
    }

    public class BloodPressureDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public string? Source { get; set; }
    }

    public class TemperatureDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public string Unit { get; set; } = string.Empty; // Celsius, Fahrenheit
        public string? Source { get; set; }
    }

    public class OxygenSaturationDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public double Percentage { get; set; }
        public string? Source { get; set; }
    }

    public class VitalSignsSummaryResponse
    {
        public int? AverageRestingHeartRate { get; set; }
        public int? AverageActiveHeartRate { get; set; }
        public double? AverageBodyTemperature { get; set; }
        public double? AverageOxygenSaturation { get; set; }
        public BloodPressureDataPointResponse? LatestBloodPressure { get; set; }
        public List<string> Alerts { get; set; } = new();
    }

    public class BodyMetricsResponse
    {
        public List<WeightDataPointResponse> Weight { get; set; } = new();
        public List<BodyCompositionDataPointResponse> BodyComposition { get; set; } = new();
        public BodyMetricsSummaryResponse Summary { get; set; } = new();
        public List<BodyTrendResponse> Trends { get; set; } = new();
    }

    public class WeightDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public double Weight { get; set; }
        public string Unit { get; set; } = string.Empty; // kg, lbs
        public string? Source { get; set; }
    }

    public class BodyCompositionDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public double? BodyFatPercentage { get; set; }
        public double? MuscleMass { get; set; }
        public double? BoneMass { get; set; }
        public double? WaterPercentage { get; set; }
        public double? VisceralFat { get; set; }
        public double? BMI { get; set; }
        public string? Source { get; set; }
    }

    public class BodyMetricsSummaryResponse
    {
        public double? CurrentWeight { get; set; }
        public double? WeightChange { get; set; }
        public string? WeightTrend { get; set; }
        public double? CurrentBMI { get; set; }
        public double? CurrentBodyFatPercentage { get; set; }
        public double? GoalWeight { get; set; }
        public double? WeightToGoal { get; set; }
    }

    public class BodyTrendResponse
    {
        public string Metric { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty; // Increasing, Decreasing, Stable
        public double ChangePercentage { get; set; }
        public string Period { get; set; } = string.Empty;
    }

    public class SleepMetricsResponse
    {
        public List<SleepSessionResponse> SleepSessions { get; set; } = new();
        public SleepSummaryResponse Summary { get; set; } = new();
        public List<SleepTrendResponse> Trends { get; set; } = new();
    }

    public class SleepSessionResponse
    {
        public DateTime BedTime { get; set; }
        public DateTime WakeTime { get; set; }
        public TimeSpan TotalSleepTime { get; set; }
        public TimeSpan TimeInBed { get; set; }
        public double SleepEfficiency { get; set; }
        public List<SleepStageResponse> SleepStages { get; set; } = new();
        public int? SleepScore { get; set; }
        public string? Source { get; set; }
    }

    public class SleepStageResponse
    {
        public string Stage { get; set; } = string.Empty; // Awake, Light, Deep, REM
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class SleepSummaryResponse
    {
        public TimeSpan AverageSleepDuration { get; set; }
        public double AverageSleepEfficiency { get; set; }
        public TimeSpan AverageBedTime { get; set; }
        public TimeSpan AverageWakeTime { get; set; }
        public int? AverageSleepScore { get; set; }
        public int ConsistentNights { get; set; }
        public List<string> SleepInsights { get; set; } = new();
    }

    public class SleepTrendResponse
    {
        public string Metric { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty;
        public double ChangeValue { get; set; }
        public string Period { get; set; } = string.Empty;
    }

    public class NutritionMetricsResponse
    {
        public List<DailyNutritionResponse> DailyNutrition { get; set; } = new();
        public NutritionSummaryResponse Summary { get; set; } = new();
        public List<HydrationDataPointResponse> Hydration { get; set; } = new();
    }

    public class DailyNutritionResponse
    {
        public DateTime Date { get; set; }
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Carbohydrates { get; set; }
        public double Fat { get; set; }
        public double Fiber { get; set; }
        public double Sugar { get; set; }
        public double Sodium { get; set; }
        public List<MealResponse> Meals { get; set; } = new();
        public string? Source { get; set; }
    }

    public class MealResponse
    {
        public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Dinner, Snack
        public DateTime Time { get; set; }
        public int Calories { get; set; }
        public List<string> Foods { get; set; } = new();
    }

    public class NutritionSummaryResponse
    {
        public int AverageDailyCalories { get; set; }
        public double AverageProtein { get; set; }
        public double AverageCarbs { get; set; }
        public double AverageFat { get; set; }
        public int LoggedDays { get; set; }
        public List<string> NutritionInsights { get; set; } = new();
    }

    public class HydrationDataPointResponse
    {
        public DateTime Timestamp { get; set; }
        public double Volume { get; set; }
        public string Unit { get; set; } = string.Empty; // ml, oz
        public string? BeverageType { get; set; }
        public string? Source { get; set; }
    }

    public class HealthTrendResponse
    {
        public string Metric { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty;
        public double ChangeValue { get; set; }
        public string ChangeDescription { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string Significance { get; set; } = string.Empty;
    }

    public class ManualHealthDataResponse
    {
        public int DataId { get; set; }
        public string DataType { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Notes { get; set; }
        public DateTime RecordedAt { get; set; }
    }

    public class HeartRateDataResponse
    {
        public List<HeartRateDataPointResponse> HeartRateData { get; set; } = new();
        public HeartRateStatsResponse Stats { get; set; } = new();
        public List<HeartRateZoneResponse> Zones { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class HeartRateStatsResponse
    {
        public int? AverageRestingHR { get; set; }
        public int? AverageActiveHR { get; set; }
        public int? MaxHeartRate { get; set; }
        public int? MinHeartRate { get; set; }
        public double? HeartRateVariability { get; set; }
        public List<string> Insights { get; set; } = new();
    }

    public class HeartRateZoneResponse
    {
        public string Zone { get; set; } = string.Empty;
        public int MinBPM { get; set; }
        public int MaxBPM { get; set; }
        public TimeSpan TimeInZone { get; set; }
        public double Percentage { get; set; }
    }

    public class SleepDataResponse
    {
        public List<SleepSessionResponse> SleepSessions { get; set; } = new();
        public SleepSummaryResponse Summary { get; set; } = new();
        public SleepInsightsResponse Insights { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SleepInsightsResponse
    {
        public string SleepQualityTrend { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new();
        public Dictionary<string, double> SleepPatterns { get; set; } = new();
        public string? SleepGoalStatus { get; set; }
    }

    public class BodyCompositionDataResponse
    {
        public List<WeightDataPointResponse> WeightData { get; set; } = new();
        public List<BodyCompositionDataPointResponse> CompositionData { get; set; } = new();
        public BodyMetricsSummaryResponse Summary { get; set; } = new();
        public List<BodyTrendResponse> Trends { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class NutritionDataResponse
    {
        public List<DailyNutritionResponse> NutritionData { get; set; } = new();
        public List<HydrationDataPointResponse> HydrationData { get; set; } = new();
        public NutritionSummaryResponse Summary { get; set; } = new();
        public NutritionInsightsResponse Insights { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class NutritionInsightsResponse
    {
        public List<string> Recommendations { get; set; } = new();
        public Dictionary<string, string> NutrientTrends { get; set; } = new();
        public string? CalorieGoalStatus { get; set; }
        public double? AverageHydration { get; set; }
        public string? HydrationGoalStatus { get; set; }
    }

    public class ExportHealthDataResponse
    {
        public string ExportId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? DownloadUrl { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? FileSize { get; set; }
        public ExportSummaryResponse? Summary { get; set; }
    }

    public class ExportSummaryResponse
    {
        public int TotalDataPoints { get; set; }
        public DateTime DateRange_Start { get; set; }
        public DateTime DateRange_End { get; set; }
        public Dictionary<string, int> DataTypesCounts { get; set; } = new();
        public List<string> IncludedSources { get; set; } = new();
    }

    public class HealthPrivacySettingsResponse
    {
        public bool ShareWithDoctors { get; set; }
        public bool ShareWithFamily { get; set; }
        public bool ShareForResearch { get; set; }
        public bool AllowDataExport { get; set; }
        public List<DataSharingPermissionResponse> ThirdPartyPermissions { get; set; } = new();
        public List<string> BlockedDataTypes { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class DataSharingPermissionResponse
    {
        public string ServiceName { get; set; } = string.Empty;
        public List<string> AllowedDataTypes { get; set; } = new();
        public DateTime PermissionGrantedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateHealthPrivacySettingsResponse
    {
        public int UserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> UpdatedSettings { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class HealthInsightsResponse
    {
        public string Timeframe { get; set; } = string.Empty;
        public OverallHealthScoreResponse OverallScore { get; set; } = new();
        public List<HealthInsightResponse> Insights { get; set; } = new();
        public List<HealthRecommendationResponse> Recommendations { get; set; } = new();
        public List<HealthAlertResponse> Alerts { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    public class OverallHealthScoreResponse
    {
        public int Score { get; set; } // 0-100
        public string Grade { get; set; } = string.Empty; // A, B, C, D, F
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, int> CategoryScores { get; set; } = new();
        public string Trend { get; set; } = string.Empty;
        public int PreviousScore { get; set; }
    }

    public class HealthInsightResponse
    {
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty; // High, Medium, Low
        public string Type { get; set; } = string.Empty; // Positive, Negative, Neutral
        public Dictionary<string, object> Data { get; set; } = new();
    }

    public class HealthRecommendationResponse
    {
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty; // High, Medium, Low
        public List<string> ActionSteps { get; set; } = new();
        public string? ExpectedBenefit { get; set; }
        public int ConfidenceScore { get; set; } // 0-100
    }

    public class HealthAlertResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // Critical, Warning, Info
        public DateTime DetectedAt { get; set; }
        public bool RequiresAction { get; set; }
        public string? RecommendedAction { get; set; }
    }

    // Placeholder command and query classes (to be implemented in Application layer)
    public class GetConnectedSourcesQuery : IRequest<ConnectedSourcesResponse>
    {
        public int UserId { get; set; }
    }

    public class ConnectDataSourceCommand : IRequest<ConnectSourceResponse>
    {
        public int UserId { get; set; }
        public string SourceType { get; set; } = string.Empty; // HealthKit, GoogleFit, Fitbit, Garmin
        public string? AuthorizationCode { get; set; }
        public List<string> RequestedPermissions { get; set; } = new();
        public Dictionary<string, string> Configuration { get; set; } = new();
    }

    public class DisconnectDataSourceCommand : IRequest
    {
        public int UserId { get; set; }
        public int SourceId { get; set; }
        public string? Reason { get; set; }
    }

    public class SyncHealthDataCommand : IRequest<SyncHealthDataResponse>
    {
        public int UserId { get; set; }
        public List<int> SourceIds { get; set; } = new(); // Empty = sync all
        public bool ForceFullSync { get; set; } = false;
    }

    public class GetHealthMetricsQuery : IRequest<HealthMetricsResponse>
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Granularity { get; set; } = "day";
    }

    public class LogManualHealthDataCommand : IRequest<ManualHealthDataResponse>
    {
        public int UserId { get; set; }
        public string DataType { get; set; } = string.Empty; // Weight, BloodPressure, HeartRate, etc.
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; }
        public string? Notes { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class GetHeartRateDataQuery : IRequest<HeartRateDataResponse>
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DataType { get; set; } = "all";
    }

    public class GetSleepDataQuery : IRequest<SleepDataResponse>
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetBodyCompositionDataQuery : IRequest<BodyCompositionDataResponse>
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeComposition { get; set; } = true;
    }

    public class GetNutritionDataQuery : IRequest<NutritionDataResponse>
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeHydration { get; set; } = true;
    }

    public class ExportHealthDataCommand : IRequest<ExportHealthDataResponse>
    {
        public int UserId { get; set; }
        public string Format { get; set; } = "json";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeRawData { get; set; } = false;
        public List<string> DataTypes { get; set; } = new();
        public List<int> SourceIds { get; set; } = new();
    }

    public class GetHealthPrivacySettingsQuery : IRequest<HealthPrivacySettingsResponse>
    {
        public int UserId { get; set; }
    }

    public class UpdateHealthPrivacySettingsCommand : IRequest<UpdateHealthPrivacySettingsResponse>
    {
        public int UserId { get; set; }
        public bool? ShareWithDoctors { get; set; }
        public bool? ShareWithFamily { get; set; }
        public bool? ShareForResearch { get; set; }
        public bool? AllowDataExport { get; set; }
        public List<string>? BlockedDataTypes { get; set; }
        public List<DataSharingPermissionRequest>? ThirdPartyPermissions { get; set; }
    }

    public class DataSharingPermissionRequest
    {
        public string ServiceName { get; set; } = string.Empty;
        public List<string> AllowedDataTypes { get; set; } = new();
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class GetHealthInsightsQuery : IRequest<HealthInsightsResponse>
    {
        public int UserId { get; set; }
        public string Timeframe { get; set; } = "month";
        public bool IncludeRecommendations { get; set; } = true;
    }
}
