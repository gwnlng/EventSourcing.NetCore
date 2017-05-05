﻿using EventSourcing.Sample.Tasks.Contracts.Accounts.Events;
using EventSourcing.Sample.Tasks.Contracts.Transactions.Events;
using System;

namespace EventSourcing.Sample.Transactions.Views.Accounts.AccountSummary
{
    public class AccountSummaryView
    {
        public Guid Id { get; private set; }

        public Guid AccountId { get; private set; }
        public Guid ClientId { get; private set; }
        public string Number { get; private set; }
        public decimal Balance { get; private set; }
        public int TransactionsCount { get; private set; }

        public void ApplyEvent(NewAccountCreated @event)
        {
            AccountId = @event.AccountId;
            Balance = 0;
            ClientId = @event.ClientId;
            Number = @event.Number;
            TransactionsCount = 0;
        }

        public void ApplyEvent(NewInflowRecorded @event)
        {
            Balance += @event.Inflow.Ammount;
        }

        public void ApplyEvent(NewOutflowRecorded @event)
        {
            Balance -= @event.Outflow.Ammount;
        }
    }
}
