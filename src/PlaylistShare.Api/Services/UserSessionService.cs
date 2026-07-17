using PlaylistShare.Api.Data;
using PlaylistShare.Api.Entities;

public class UserSessionService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSessionService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserSession> GetOrCreateCurrentSessionAsync(Guid? associatedUserId = null, CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext available");

        var sessionId = httpContext.Session.Id;
        var now = DateTime.UtcNow;

        var session = await _db.UserSessions.FindAsync([sessionId], cancellationToken);
        if (session == null)
        {
            session = new UserSession
            {
                SessionId = sessionId,
                ClientIpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                FirstSeenUtc = now,
                LastSeenUtc = now,
                AssociatedUserId = associatedUserId
            };
            _db.UserSessions.Add(session);
        }
        else
        {
            session.LastSeenUtc = now;
            if (session.AssociatedUserId == null && associatedUserId != null)
                session.AssociatedUserId = associatedUserId;
            _db.UserSessions.Update(session);
        }
        await _db.SaveChangesAsync(cancellationToken);
        return session;
    }
}