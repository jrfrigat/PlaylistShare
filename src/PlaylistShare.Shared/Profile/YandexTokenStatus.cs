namespace PlaylistShare.Shared.Profile;

public class YandexTokenStatus
{
    public bool HasToken { get; set; }
    public bool IsValid { get; set; }
    public DateTime ExpiryUtc { get; set; }
}