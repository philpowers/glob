using System.Collections.Generic;
using System.Linq;

namespace Glob
{
    static class Lst
    {
        public static Lst<T> ToLst<T>(this IEnumerable<T> items) =>
            items.Reverse().Aggregate(Nil<T>.Instance, (list, item) => list.Prepend(item));
    }

    abstract class Lst<T>
    {
        public Lst<T> Prepend(T head) => new Cons<T>(head, this);
    }

    class Cons<T> : Lst<T>
    {
        public Cons(T head, Lst<T> tail)
        {
            this.Head = head;
            this.Tail = tail;
        }

        public T Head { get; }
        public Lst<T> Tail { get; }

        public void Deconstruct(out T head, out Lst<T> tail)
        {
            head = this.Head;
            tail = this.Tail;
        }
    }

    class Nil<T> : Lst<T>
    {
        public static readonly Lst<T> Instance = new Nil<T>();
        private Nil()
        {
        }
    }
}