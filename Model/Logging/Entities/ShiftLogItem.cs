using Cashbox.Model;
using Cashbox.Model.Entities;
using OfficeOpenXml.Attributes;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Cashbox.Model.Logging.Entities
{
    [EpplusTable(PrintHeaders = false, AutofitColumns = true)]
    public class ShiftLogItem : ILogItem
    {
        public ShiftLogItem(Shift entity)
        {
            Cash = entity.Cash;
            Comment = entity.Comment;
            CreatedAt = Formatter.FormatDate(entity.CreatedAt);
            Difference = entity.Difference;
            EndDay = entity.EndDay;
            Expenses = entity.Expenses;
            HandedOver = entity.HandedOver;
            LastModified = entity.LastModified.ToString();
            StartDay = entity.StartDay;
            Terminal = entity.Terminal;
            Total = entity.Total;
            Version = entity.Version;
        }

        [EpplusTableColumn(Order = 0)]
        [Description("Версия смены")]
        public int Version { get; set; }

        [EpplusTableColumn(Order = 1)]
        [Description("Дата создания")]
        public string CreatedAt { get; set; }

        [EpplusTableColumn(Order = 2)]
        [Description("Дата и время последнего изменения")]
        public string LastModified { get; set; }

        [EpplusTableColumn(Order = 3)]
        [Description("Наличные")]
        public int Cash { get; set; }

        [EpplusTableColumn(Order = 4)]
        [Description("Терминал")]
        public int Terminal { get; set; }

        [EpplusTableColumn(Order = 5)]
        [Description("Расходы")]
        public int Expenses { get; set; }

        [EpplusTableColumn(Order = 6)]
        [Description("Сумма на начало дня")]
        public int StartDay { get; set; }

        [EpplusTableColumn(Order = 7)]
        [Description("Сумма на конец дня")]
        public int EndDay { get; set; }

        [EpplusTableColumn(Order = 8)]
        [Description("Сдано денег")]
        public int HandedOver { get; set; }

        [EpplusTableColumn(Order = 9)]
        [Description("Общая выручка")]
        public int Total { get; set; }

        [EpplusTableColumn(Order = 10)]
        [Description("Расхождение")]
        public int Difference { get; set; }

        [EpplusTableColumn(Order = 11)]
        [Description("Комментарий")]
        public string Comment { get; set; }

        public int GetMessageId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.Create => 1,
                MessageType.Update => 2,
                MessageType.Delete => 3,
                _ => 0
            };
        }

        public Dictionary<string, object> GetPropertiesInfo()
        {
            return Util.GetPropertiesInfo(this);
        }
    }
}