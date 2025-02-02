﻿namespace BuildingBlocks.Domain
{
    public abstract class Validator<T> : IValidator where T : class
    {
        protected readonly IValidatorNotificationHandler _notificationHandler;
        protected readonly T _entity;

        protected Validator(T entity, IValidatorNotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
            _entity = entity;
        }


        public abstract bool Validate();
    }
}
