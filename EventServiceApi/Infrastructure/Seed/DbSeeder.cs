using EventServiceApi.Data;
using EventServiceApi.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Infrastructure.Seed;

/// <summary>
/// Класс для заполнения базы данных тестовыми данными
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Заполнить БД тестовыми событиями (если она пуста)
    /// </summary>
    public static async Task SeedEventsAsync(AppDbContext context)
    {
        // Не дублируем данные при повторных запусках
        if (await context.Events.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var today = now.Date;

        var events = new List<Event>
        {
            new(
                "Ежегодная IT-конференция DevCon 2026",
                "Крупнейшая конференция для разработчиков: доклады, воркшопы и нетворкинг.",
                today.AddDays(7).AddHours(9),
                today.AddDays(7).AddHours(18)),

            new(
                "Воркшоп по C# и .NET 10",
                "Практический воркшоп: новые возможности языка и платформы.",
                today.AddDays(3).AddHours(10),
                today.AddDays(3).AddHours(17)),

            new(
                "Митинг фронтенд-разработчиков",
                "Обсуждаем React, Vue, Svelte и тренды фронтенда.",
                today.AddDays(14).AddHours(18),
                today.AddDays(14).AddHours(21)),

            new(
                "Хакатон «AI for Good»",
                "48 часов на создание AI-решений для социальных задач.",
                today.AddDays(21).AddHours(9),
                today.AddDays(23).AddHours(18)),

            new(
                "Курс по архитектуре микросервисов",
                "Интенсив по проектированию распределённых систем.",
                today.AddDays(1).AddHours(9),
                today.AddDays(5).AddHours(18)),

            new(
                "Вебинар: Docker и Kubernetes в проде",
                null, 
                today.AddDays(2).AddHours(15),
                today.AddDays(2).AddHours(17)),

            new(
                "Встреча книжного клуба программистов",
                "Обсуждаем «Чистый код» Роберта Мартина.",
                today.AddDays(10).AddHours(19),
                today.AddDays(10).AddHours(21)),

            new(
                "Конференция по кибербезопасности SecConf",
                "Актуальные угрозы, пентест и защита инфраструктуры.",
                today.AddDays(30).AddHours(9),
                today.AddDays(30).AddHours(18)),

            new(
                "Практикум по тестированию в .NET",
                "xUnit, NSubstitute, интеграционные тесты и TestContainers.",
                today.AddDays(5).AddHours(11),
                today.AddDays(5).AddHours(16)),

            new(
                "Онлайн-митап по базам данных",
                "PostgreSQL vs MS SQL vs MongoDB — что выбрать в 2026?",
                today.AddDays(4).AddHours(18),
                today.AddDays(4).AddHours(20)),

            new(
                "Семинар по DevOps-практикам",
                "CI/CD, IaC, мониторинг и observability.",
                today.AddDays(12).AddHours(10),
                today.AddDays(12).AddHours(17)),

            new(
                "Пикник IT-сообщества",
                "Неформальная встреча для общения и обмена опытом.",
                today.AddDays(6).AddHours(12),
                today.AddDays(6).AddHours(18)),

            new(
                "Итоги года: ретроспектива технологий",
                "Что изменилось в 2026 и что ждёт нас в 2027.",
                today.AddDays(45).AddHours(17),
                today.AddDays(45).AddHours(19)),

            new(
                "Мастер-класс по Blazor",
                "Создаём SPA на C# без JavaScript — от идеи до деплоя.",
                today.AddDays(9).AddHours(14),
                today.AddDays(9).AddHours(18))
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}