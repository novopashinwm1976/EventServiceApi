using System.Net;

namespace EventServiceApi.Presentation;

//Честно говоря не решил куда этот класс поместить - поскольку он работает с контроллером, то оставил тут

//Класс ApiResult c возвращаемыми данными
//Наследуемся от базового класса с основными параметрами

/// <summary>
/// Класс ApiResult c возвращаемыми данными
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiResult<T> : ApiBaseResult
{
    /// <summary>
    /// Возвращаемые данные метода
    /// </summary>
    public required T Data { get; set; }
}


/// <summary>
/// Класс ApiResult без возвращаемых данных
/// Наследуемся от базового класса с основными параметрами
/// </summary>
public class ApiResult : ApiBaseResult { }

/// <summary>
/// Базовый класс с основными параметрами
/// </summary>
public class ApiBaseResult
{
    /// <summary>
    /// Флаг, указывающий на успешность выполненного запроса
    /// </summary>
    public required bool Success { get; set; }

    /// <summary>
    /// Возвращаемый HTTP код
    /// </summary>
    public required HttpStatusCode StatusCode { get; set; }
    /// <summary>
    /// Дата и время ответа
    /// </summary>
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Кастомное сообщение с дополнительной информацией
    /// </summary>
    public required string Message { get; set; }
}
