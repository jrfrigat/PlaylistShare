namespace PlaylistShare.Api.Extensions;

public static class HttpContextExtensions
{
    // Ключ и значение якоря. Их НИКТО не читает, и это намеренно - смысл записи не в значении.
    private const string SessionAnchorKey = "anchor";
    private const string SessionAnchorValue = "1";

    /// <summary>
    /// Возвращает id сессии, который переживает границу запроса.
    /// </summary>
    /// <remarks>
    /// НЕ УДАЛЯЙТЕ запись якоря, какой бы бессмысленной она ни выглядела: без неё id разный на
    /// каждый запрос, и права RemovePermission.AddedByUserOnly для анонима перестают работать
    /// совсем (трек добавлен под одним id, удаляется под другим).
    ///
    /// Причина в устройстве ASP.NET Core. Cookie сессии ставится только если вызван
    /// tryEstablishSession, а его дёргает единственный метод - ISession.Set. Чтение Session.Id
    /// сессию не устанавливает, поэтому без записи cookie не уходит и следующий запрос приходит
    /// без неё - middleware заводит новый ключ. Хуже того, сам Session.Id хранится ВНУТРИ
    /// сериализованной сессии: если в хранилище по ключу пусто, id генерируется случайным заново,
    /// так что одной cookie тоже мало - в хранилище должно что-то лежать.
    ///
    /// Отсюда якорь: любое значение, лишь бы Set был вызван и сессия попала в хранилище. Пишем
    /// только один раз - на последующих запросах значение уже загружено, лишняя запись не нужна.
    ///
    /// Вызывать нужно до начала отправки ответа, иначе cookie в него уже не добавить.
    /// </remarks>
    public static string GetStableSessionId(this HttpContext context)
    {
        if (context.Session.GetString(SessionAnchorKey) is null)
            context.Session.SetString(SessionAnchorKey, SessionAnchorValue);

        return context.Session.Id;
    }
}
