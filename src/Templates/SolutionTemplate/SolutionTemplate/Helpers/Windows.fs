namespace SolutionTemplate.WinRT

/// Specifies the execution state of the app.
type ApplicationExecutionState =
    /// The app is not running.
    | NotRunning = 0
    /// The app is running.
    | Running = 1
    /// The app is suspended.
    | Suspended = 2
    /// The app was terminated after being suspended.
    | Terminated = 3
    /// The app was closed by the user.
    | ClosedByUser = 4

/// Defines the level of connectivity currently available.
type NetworkConnectivityLevel =
    /// No connectivity.
    | None = 0
    /// Local network access only.
    | LocalAccess = 1
    /// Limited internet access.
    | ConstrainedInternetAccess = 2
    /// Local and Internet access.
    | InternetAccess = 3

