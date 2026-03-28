using Qute.Directus.Http;
using Qute.Directus.Models;
using Qute.Directus.Models.Users;

namespace Qute.Directus.Services;

/// <summary>
/// Service for Directus user operations.
/// </summary>
public sealed class UsersService
{
    private readonly DirectusHttpClient _http;

    public UsersService(DirectusHttpClient http) => _http = http;

    // ─── Read ──────────────────────────────────────────────────────────

    /// <summary>List all users.</summary>
    public Task<DirectusListResponse<DirectusUser>> GetManyAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetListAsync<DirectusUser>("users", query, ct);

    /// <summary>List all users with a query builder.</summary>
    public Task<DirectusListResponse<DirectusUser>> GetManyAsync(Action<QueryParameters> configure, CancellationToken ct = default)
    {
        var q = new QueryParameters();
        configure(q);
        return GetManyAsync(q, ct);
    }

    /// <summary>Retrieve a single user by ID.</summary>
    public Task<DirectusUser> GetByIdAsync(string id, QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusUser>($"users/{id}", query, ct);

    /// <summary>Retrieve the currently authenticated user.</summary>
    public Task<DirectusUser> GetCurrentAsync(QueryParameters? query = null, CancellationToken ct = default)
        => _http.GetAsync<DirectusUser>("users/me", query, ct);

    // ─── Create ────────────────────────────────────────────────────────

    /// <summary>Create a single user.</summary>
    public Task<DirectusUser> CreateAsync(DirectusUser user, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostAsync<DirectusUser>("users", user, query, ct);

    /// <summary>Create multiple users.</summary>
    public Task<DirectusListResponse<DirectusUser>> CreateManyAsync(IEnumerable<DirectusUser> users, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PostListAsync<DirectusUser>("users", users, query, ct);

    // ─── Update ────────────────────────────────────────────────────────

    /// <summary>Update a single user by ID.</summary>
    public Task<DirectusUser> UpdateAsync(string id, object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusUser>($"users/{id}", data, query, ct);

    /// <summary>Update multiple users at once.</summary>
    public Task<DirectusListResponse<DirectusUser>> UpdateManyAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchListAsync<DirectusUser>("users", data, query, ct);

    /// <summary>Update the currently authenticated user.</summary>
    public Task<DirectusUser> UpdateCurrentAsync(object data, QueryParameters? query = null, CancellationToken ct = default)
        => _http.PatchAsync<DirectusUser>("users/me", data, query, ct);

    // ─── Delete ────────────────────────────────────────────────────────

    /// <summary>Delete a single user by ID.</summary>
    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _http.DeleteAsync($"users/{id}", ct);

    /// <summary>Delete multiple users by IDs.</summary>
    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken ct = default)
        => _http.DeleteAsync("users", ids, ct);

    // ─── Invite ────────────────────────────────────────────────────────

    /// <summary>Invite a user by email.</summary>
    public Task InviteAsync(string email, string role, string? inviteUrl = null, CancellationToken ct = default)
        => _http.PostAsync("users/invite", new UserInviteRequest { Email = email, Role = role, InviteUrl = inviteUrl }, ct);

    /// <summary>Accept a user invitation.</summary>
    public Task AcceptInviteAsync(string token, string password, CancellationToken ct = default)
        => _http.PostAsync("users/invite/accept", new AcceptInviteRequest { Token = token, Password = password }, ct);

    // ─── Register ──────────────────────────────────────────────────────

    /// <summary>Register a new user (requires public registration to be enabled).</summary>
    public Task RegisterAsync(RegisterUserRequest request, CancellationToken ct = default)
        => _http.PostAsync("users/register", request, ct);

    /// <summary>Verify a registered user's email.</summary>
    public Task VerifyEmailAsync(string token, CancellationToken ct = default)
        => _http.GetAsync<object>($"users/register/verify-email/{token}", ct: ct);

    // ─── TFA ───────────────────────────────────────────────────────────

    /// <summary>Generate a TFA secret for the current user.</summary>
    public Task<TfaGenerateResponse> GenerateTfaSecretAsync(string password, CancellationToken ct = default)
        => _http.PostAuthenticatedRawAsync<TfaGenerateResponse>("users/me/tfa/generate", new { password }, ct);

    /// <summary>Enable TFA for the current user.</summary>
    public Task EnableTfaAsync(string secret, string otp, CancellationToken ct = default)
        => _http.PostAsync("users/me/tfa/enable", new TfaEnableRequest { Secret = secret, Otp = otp }, ct);

    /// <summary>Disable TFA for the current user.</summary>
    public Task DisableTfaAsync(string otp, CancellationToken ct = default)
        => _http.PostAsync("users/me/tfa/disable", new TfaDisableRequest { Otp = otp }, ct);

    // ─── Track Page ────────────────────────────────────────────────────

    /// <summary>Update the last visited page for the current user.</summary>
    public Task UpdateLastPageAsync(string lastPage, CancellationToken ct = default)
        => _http.PatchAsync<object>("users/me/track/page", new { last_page = lastPage }, ct: ct);
}
