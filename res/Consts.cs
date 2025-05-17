using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TiketsApp.res
{
    internal sealed class Consts
    {
       public const string SqlConnection = @"Server=DESKTOP-HJGN3L3;Database=tickets;Trusted_Connection=True;TrustServerCertificate=True;" +
            "Pooling=true;" +
                      "Min Pool Size=5;" +
                      "Max Pool Size=100;" +
                      "Connection Lifetime=300;" +
                      "Connection Timeout=30;";

        public const string PassError = """
            Минимум 8 символов.
            Латиница. 
            Минимум 1 прописная буква, цифра и символ из: !@#$&*
            """;

        public const string EmailError = "Неверный формат Email";

        public const string NameError = "Недопустимы пробелы, цифры и спецсимволы";

        public const string ReqiredMessage = "Обязательное поле";

        public const string NumberMessage = "Номер из 10 цифр";

        public const string CategoryLenghtMessage = "Максимум 30 знаков";

        public const string EventLenghtMessage = "Максимум 33 знаков";

        public const string EventDescriptionMessage = "Максимум 200 знаков";


        public const string PasswordPattern = @"^(?=.*[A-Z].*)(?=.*[!@#$&*])(?=.*[0-9].*)(?=.*[a-z].*).{8,}$";

        public const string FioPattern = @"(^[a-zA-Z]+$)|(^[а-яА-Я]+$)";

        public const string NumPattern = @"^\d{10}$";

        public static readonly IReadOnlyList<string> EmailMessage = ["Email занят"];

        public static readonly IReadOnlyList<string> SallerIdMessage = ["Данный идентификатор уже зарегистрирован"];

        public static readonly IReadOnlyList<string> EmailNotFoundMsg = ["Email не верен"];

        public static readonly IReadOnlyList<string> PassNotFoundMsg = ["Пароль не верен"];

        public static readonly IReadOnlyList<string> BannedMsg = ["Пользователь забанен"];

        public static readonly IReadOnlyList<string> AccauntNotAddedMsg = ["Ваша регистрация пока не одобрена"];

        public static readonly IReadOnlyList<string> CategoryExistsMsg = ["Эта категория уже существует"];

        public const string RepeatPassError = "Пароли не совпадают";
    }
}
