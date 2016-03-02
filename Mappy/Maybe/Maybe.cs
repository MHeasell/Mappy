namespace Mappy.Maybe
{
    using System;
    using System.Collections.Generic;

    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly bool hasValue;

        private readonly T value;

        internal Maybe(T value)
        {
            this.value = value;
            this.hasValue = this.value != null;
        }

        public bool IsNone => !this.hasValue;

        public bool HasValue => this.hasValue;

        public bool IsSome => this.hasValue;

        public T UnsafeValue => this.value;

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
            if (this.hasValue && other.hasValue)
            {
                return EqualityComparer<T>.Default.Equals(this.value, other.value);
            }

            return this.hasValue == other.hasValue;
        }

        public override bool Equals(object obj)
        {
            return (obj is Maybe<T>) && this.Equals((Maybe<T>)obj);
        }

        public override int GetHashCode()
        {
            return this.hasValue ? this.value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return this.hasValue ? $"Some({this.value})" : "None";
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

            return this.hasValue ? some(this.value) : none();
        }

        public void Match(Action<T> some, Action none)
        {
            if (some == null)
            {
                throw new ArgumentNullException(nameof(some));
            }

            if (none == null)
            {
                throw new ArgumentNullException(nameof(none));
            }

            if (this.hasValue)
            {
                some(this.value);
            }
            else
            {
                none();
            }
        }

        public Maybe<T> Or(Maybe<T> other)
        {
            return this.hasValue ? this : other;
        }

        public T Or(T otherValue)
        {
            return this.hasValue ? this.value : otherValue;
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

            return this.hasValue ? new Maybe<TR>(f(this.value)) : Maybe.None<TR>();
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

            return this.hasValue ? f(this.value) : Maybe.None<TR>();
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

            return this.hasValue && filter(this.value) ? this : Maybe.None<T>();
        }

        public Maybe<T> Filter(Predicate<T> filter)
        {
            return this.Where(filter);
        }

        public void IfSome(Action<T> action)
        {
            if (this.hasValue)
            {
                action(this.value);
            }
        }

        public void IfNone(Action action)
        {
            if (!this.hasValue)
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
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> None<T>()
        {
            return default(Maybe<T>);
        }
    }
}
