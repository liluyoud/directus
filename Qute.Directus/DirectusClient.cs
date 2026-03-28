using Qute.Directus.Http;
using Qute.Directus.Services;

namespace Qute.Directus;

/// <summary>
/// Main entry point for interacting with the Directus API.
/// Provides access to all Directus services through strongly-typed properties.
/// </summary>
/// <remarks>
/// <para>
/// Usage with DI:
/// <code>
/// services.AddDirectus(options => {
///     options.BaseUrl = "https://my-directus.com";
///     options.StaticToken = "my-token";
/// });
/// </code>
/// </para>
/// <para>
/// Usage without DI:
/// <code>
/// var client = new DirectusClient(new DirectusOptions { BaseUrl = "https://my-directus.com" });
/// await client.Auth.LoginAsync("admin@example.com", "password");
/// var users = await client.Users.GetManyAsync();
/// </code>
/// </para>
/// </remarks>
public sealed class DirectusClient : IDisposable
{
    private readonly TokenManager _tokenManager;
    private readonly DirectusHttpClient _http;
    private readonly HttpClient? _ownedHttpClient;

    // ─── Lazy service instances ────────────────────────────────────────

    private AuthenticationService? _auth;
    private ItemsService? _items;
    private UsersService? _users;
    private FilesService? _files;
    private AssetsService? _assets;
    private CollectionsService? _collections;
    private FieldsService? _fields;
    private RolesService? _roles;
    private PermissionsService? _permissions;
    private PoliciesService? _policies;
    private ActivityService? _activity;
    private FoldersService? _folders;
    private RelationsService? _relations;
    private RevisionsService? _revisions;
    private PresetsService? _presets;
    private SettingsService? _settings;
    private ServerService? _server;
    private FlowsService? _flows;
    private OperationsService? _operations;
    private DashboardsService? _dashboards;
    private PanelsService? _panels;
    private NotificationsService? _notifications;
    private SharesService? _shares;
    private TranslationsService? _translations;
    private CommentsService? _comments;
    private ExtensionsService? _extensions;
    private VersionsService? _versions;
    private SchemaService? _schema;
    private UtilitiesService? _utilities;
    private MetricsService? _metrics;

    /// <summary>
    /// Creates a new <see cref="DirectusClient"/> using the provided options and an internally managed HttpClient.
    /// </summary>
    public DirectusClient(DirectusOptions options)
    {
        _tokenManager = new TokenManager(options.TokenRefreshBufferSeconds);
        _ownedHttpClient = new HttpClient { BaseAddress = options.GetBaseUri() };
        _http = new DirectusHttpClient(_ownedHttpClient, _tokenManager, options);
    }

    /// <summary>
    /// Creates a new <see cref="DirectusClient"/> using the provided HttpClient (for DI / IHttpClientFactory usage).
    /// </summary>
    public DirectusClient(HttpClient httpClient, DirectusOptions options)
    {
        _tokenManager = new TokenManager(options.TokenRefreshBufferSeconds);
        _http = new DirectusHttpClient(httpClient, _tokenManager, options);
    }

    /// <summary>
    /// Creates a new <see cref="DirectusClient"/> with pre-built dependencies (used internally by DI).
    /// </summary>
    internal DirectusClient(DirectusHttpClient http, TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
        _http = http;
    }

    // ─── Service Properties ────────────────────────────────────────────

    /// <summary>Authentication (login, logout, refresh, password reset, OAuth).</summary>
    public AuthenticationService Auth => _auth ??= new AuthenticationService(_http, _tokenManager);

    /// <summary>Generic CRUD operations on collection items.</summary>
    public ItemsService Items => _items ??= new ItemsService(_http);

    /// <summary>User management (CRUD, invite, register, TFA, current user).</summary>
    public UsersService Users => _users ??= new UsersService(_http);

    /// <summary>File management (upload, import, CRUD).</summary>
    public FilesService Files => _files ??= new FilesService(_http);

    /// <summary>Asset retrieval with optional image transformations.</summary>
    public AssetsService Assets => _assets ??= new AssetsService(_http);

    /// <summary>Collection management (CRUD).</summary>
    public CollectionsService Collections => _collections ??= new CollectionsService(_http);

    /// <summary>Field management (CRUD per collection).</summary>
    public FieldsService Fields => _fields ??= new FieldsService(_http);

    /// <summary>Role management (CRUD).</summary>
    public RolesService Roles => _roles ??= new RolesService(_http);

    /// <summary>Permission management (CRUD).</summary>
    public PermissionsService Permissions => _permissions ??= new PermissionsService(_http);

    /// <summary>Access policy management (CRUD).</summary>
    public PoliciesService Policies => _policies ??= new PoliciesService(_http);

    /// <summary>Activity log (read-only).</summary>
    public ActivityService Activity => _activity ??= new ActivityService(_http);

    /// <summary>Virtual folder management (CRUD).</summary>
    public FoldersService Folders => _folders ??= new FoldersService(_http);

    /// <summary>Relation management (CRUD).</summary>
    public RelationsService Relations => _relations ??= new RelationsService(_http);

    /// <summary>Revision log (read-only).</summary>
    public RevisionsService Revisions => _revisions ??= new RevisionsService(_http);

    /// <summary>Preset management (saved views, CRUD).</summary>
    public PresetsService Presets => _presets ??= new PresetsService(_http);

    /// <summary>Global settings (get/update singleton).</summary>
    public SettingsService Settings => _settings ??= new SettingsService(_http);

    /// <summary>Server info, health, and ping.</summary>
    public ServerService Server => _server ??= new ServerService(_http);

    /// <summary>Automation Flow management (CRUD + trigger).</summary>
    public FlowsService Flows => _flows ??= new FlowsService(_http);

    /// <summary>Flow operation/step management (CRUD).</summary>
    public OperationsService Operations => _operations ??= new OperationsService(_http);

    /// <summary>Dashboard management (CRUD).</summary>
    public DashboardsService Dashboards => _dashboards ??= new DashboardsService(_http);

    /// <summary>Panel (dashboard widget) management (CRUD).</summary>
    public PanelsService Panels => _panels ??= new PanelsService(_http);

    /// <summary>Notification management (CRUD).</summary>
    public NotificationsService Notifications => _notifications ??= new NotificationsService(_http);

    /// <summary>Share management (CRUD + info).</summary>
    public SharesService Shares => _shares ??= new SharesService(_http);

    /// <summary>Custom translation management (CRUD).</summary>
    public TranslationsService Translations => _translations ??= new TranslationsService(_http);

    /// <summary>Comment management (CRUD).</summary>
    public CommentsService Comments => _comments ??= new CommentsService(_http);

    /// <summary>Extension management (list + update).</summary>
    public ExtensionsService Extensions => _extensions ??= new ExtensionsService(_http);

    /// <summary>Content Version management (CRUD + save + promote).</summary>
    public VersionsService Versions => _versions ??= new VersionsService(_http);

    /// <summary>Schema management (snapshot, diff, apply).</summary>
    public SchemaService Schema => _schema ??= new SchemaService(_http);

    /// <summary>Utility operations (hash, sort, random string, export).</summary>
    public UtilitiesService Utilities => _utilities ??= new UtilitiesService(_http);

    /// <summary>Prometheus metrics endpoint.</summary>
    public MetricsService Metrics => _metrics ??= new MetricsService(_http);

    /// <summary>
    /// Disposes the internally managed HttpClient and TokenManager (if owned by this instance).
    /// </summary>
    public void Dispose()
    {
        _tokenManager.Dispose();
        _ownedHttpClient?.Dispose();
    }
}
