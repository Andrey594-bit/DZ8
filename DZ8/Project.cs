using System;
using System.Collections.Generic;

namespace TaskManager
{
    enum ProjectStatus { Проект, Исполнение, Закрыт }
    enum TaskStatus { Назначена, ВРаботе, НаПроверке, Выполнена }
    enum ReportStatus { НаПроверке, Утверждён, НаДоработке }

    class Person
    {
        public string Name { get; set; }
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public Person(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class Report
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public Person Author { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.НаПроверке;

        public Report(string text, Person author)
        {
            Text = text;
            Date = DateTime.Now;
            Author = author;
        }

        public override string ToString()
        {
            return $"Отчёт от {Author} ({Date:dd.MM.yyyy}) [{Status}]: {Text}";
        }
    }

    class TaskItem
    {
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Person Initiator { get; set; }
        public Person Assignee { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Назначена;
        public Report Report { get; set; }

        public TaskItem(string description, DateTime deadline, Person initiator)
        {
            Description = description;
            Deadline = deadline;
            Initiator = initiator;
        }

        public override string ToString()
        {
            string assigneeName = Assignee == null ? "—" : Assignee.Name;
            return $"\"{Description}\" | срок: {Deadline:dd.MM.yyyy} | исполнитель: {assigneeName} | статус: {Status}";
        }
    }

    class Project
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Person Customer { get; set; }
        public Person TeamLead { get; set; }
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public ProjectStatus Status { get; set; } = ProjectStatus.Проект;

        public Project(string description, DateTime start, DateTime end, Person customer, Person teamLead)
        {
            Description = description;
            StartDate = start;
            EndDate = end;
            Customer = customer;
            TeamLead = teamLead;
        }

        public void AddTask(TaskItem task)
        {
            if (Status != ProjectStatus.Проект)
                throw new InvalidOperationException("Задачи можно создавать только в статусе «Проект».");
            if (task.Initiator != TeamLead)
                throw new InvalidOperationException("Задачи назначает только тимлид проекта.");
            Tasks.Add(task);
            if (task.Assignee != null)
                task.Assignee.Tasks.Add(task);
        }

        public void RemoveTask(TaskItem task)
        {
            if (task.Assignee != null)
                throw new InvalidOperationException("Нельзя удалить задачу, у которой есть исполнитель.");
            Tasks.Remove(task);
        }

        public void Start()
        {
            if (Status != ProjectStatus.Проект)
                throw new InvalidOperationException("Проект уже запущен или закрыт.");
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Assignee == null)
                    throw new InvalidOperationException("Есть задачи без исполнителя.");
            }
            Status = ProjectStatus.Исполнение;
        }

        public void TryClose()
        {
            if (Status != ProjectStatus.Исполнение)
                throw new InvalidOperationException("Закрыть можно только проект в статусе «Исполнение».");
            if (Tasks.Count == 0)
                throw new InvalidOperationException("В проекте нет задач.");
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Status != TaskStatus.Выполнена)
                    throw new InvalidOperationException("Не все задачи выполнены.");
            }
            Status = ProjectStatus.Закрыт;
        }

        public override string ToString()
        {
            return $"Проект \"{Description}\" | {StartDate:dd.MM.yyyy}–{EndDate:dd.MM.yyyy} | заказчик: {Customer} | тимлид: {TeamLead} | статус: {Status}";
        }
    }

    static class Workflow
    {
        public static void TakeToWork(TaskItem task)
        {
            if (task.Status != TaskStatus.Назначена)
                throw new InvalidOperationException("Взять в работу можно только назначенную задачу.");
            task.Status = TaskStatus.ВРаботе;
        }

        public static void Delegate(TaskItem task, Person newAssignee)
        {
            if (task.Status != TaskStatus.Назначена)
                throw new InvalidOperationException("Делегировать можно только назначенную задачу.");
            if (newAssignee == task.Assignee)
                throw new InvalidOperationException("Нельзя делегировать задачу самому себе.");
            if (task.Assignee != null)
                task.Assignee.Tasks.Remove(task);
            task.Assignee = newAssignee;
            newAssignee.Tasks.Add(task);
            task.Status = TaskStatus.Назначена;
        }

        public static void Reject(TaskItem task)
        {
            if (task.Status != TaskStatus.Назначена)
                throw new InvalidOperationException("Отклонить можно только назначенную задачу.");
            if (task.Assignee != null)
                task.Assignee.Tasks.Remove(task);
            task.Assignee = null;
            task.Status = TaskStatus.Назначена;
        }

        public static void SubmitReport(TaskItem task, Report report)
        {
            if (task.Status != TaskStatus.ВРаботе)
                throw new InvalidOperationException("Отчёт можно сдать только по задаче «В работе».");
            task.Report = report;
            task.Status = TaskStatus.НаПроверке;
        }

        public static void ApproveReport(TaskItem task, Person who)
        {
            if (who != task.Initiator)
                throw new InvalidOperationException("Утверждать отчёт может только инициатор задачи.");
            if (task.Status != TaskStatus.НаПроверке || task.Report == null)
                throw new InvalidOperationException("Нет отчёта на проверке.");
            task.Report.Status = ReportStatus.Утверждён;
            task.Status = TaskStatus.Выполнена;
        }

        public static void ReturnReport(TaskItem task, Person who)
        {
            if (who != task.Initiator)
                throw new InvalidOperationException("Возвращать отчёт может только инициатор задачи.");
            if (task.Status != TaskStatus.НаПроверке || task.Report == null)
                throw new InvalidOperationException("Нет отчёта на проверке.");
            task.Report.Status = ReportStatus.НаДоработке;
            task.Status = TaskStatus.ВРаботе;
        }
    }

}