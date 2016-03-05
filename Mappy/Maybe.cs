namespace Mappy
{
    using System;
    using System.Collections.Generic;

    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        internal Maybe(T value)
        {
            this.UnsafeValue = value;
            this.HasValue = this.UnsafeValue != null;
        }

        public bool HasValue { get; }

        public T UnsafeValue { get; }

        public bool IsNone => !this.HasValue;

        public bool IsSome => this.HasValue;

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Maybe<T> other)
        {
            if (this.HasValue && other.HasValue)
            {
                return EqualityComparer<T>.Default.Equals(this.UnsafeValue, other.UnsafeValue);
            }

            return this.HasValue == other.HasValue;
        }

        public override bool Equals(object obj)
        {
            return (obj is Maybe<T>) && this.Equals((Maybe<T>)obj);
        }

        public override int GetHashCode()
        {
            return this.HasValue ? this.UnsafeValue.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return this.HasValue ? $"Some({this.UnsafeValue})" : "None";
        }

        public TR Match<TR>(Func<T, TR> some, Func<TR> none)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }

            return this.HasValue ? some(this.UnsafeValue) : none();
        }

        public void Do(Action<T> some, Action none)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }

            if (this.HasValue)
            {
                some(this.UnsafeValue);
            }
            else
            {
                none();
            }
        }

        public Maybe<T> Or(Maybe<T> other)
        {
            return this.HasValue ? this : other;
        }

        public T Or(T otherValue)
        {
            return this.HasValue ? this.UnsafeValue : otherValue;
        }

        public T GetOrDefault(T defaultValue)
        {
            return this.Or(defaultValue);
        }

        public Maybe<TR> Map<TR>(Func<T, TR> f)
        {
            if (f == null)
            {
                throw new ArgumentNullException(nameof(f));
            }

            return this.HasValue ? new Maybe<TR>(f(this.UnsafeValue)) : Maybe.None<TR>();
        }

        public Maybe<TR> Select<TR>(Func<T, TR> f)
        {
            return this.Map(f);
        }

        public Maybe<TR> FlatMap<TR>(Func<T, Maybe<TR>> f)
        {
            if (f == null)
            {
                throw new ArgumentNullException(nameof(f));
            }

            return this.HasValue ? f(this.UnsafeValue) : Maybe.None<TR>();
        }

        public Maybe<TR> SelectMany<TR>(Func<T, Maybe<TR>> f)
        {
            return this.FlatMap(f);
        }

        public Maybe<TR> Bind<TR>(Func<T, Maybe<TR>> f)
        {
            return this.FlatMap(f);
        }

        public Maybe<T> Where(Predicate<T> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return this.HasValue && filter(this.UnsafeValue) ? this : Maybe.None<T>();
        }

        public Maybe<T> Filter(Predicate<T> filter)
        {
            return this.Where(filter);
        }

        public void IfSome(Action<T> action)
        {
            if (this.HasValue)
            {
                action(this.UnsafeValue);
            }
        }

        public void IfNone(Action action)
        {
            if (!this.HasValue)
            {
                action();
            }
        }
    }

    public static class Maybe
    {
        public static Maybe<T> Return<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return new Maybe<T>(value);
        }

        public static Maybe<T> Some<T>(T value)
        {
            return Return(value);
        }

        public static Maybe<T> From<T>(T value)
            where T : class
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> From<T>(T? value)
           where T : struct
        {
            return value.HasValue ? Some(value.Value) : None<T>();
        }

        public static Maybe<T> None<T>()
        {
            return default(Maybe<T>);
        }
    }
}
