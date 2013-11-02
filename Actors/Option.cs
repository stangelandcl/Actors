using System;

namespace Actors
{
	public struct Option
	{
		public static Option<T> None<T>(){
			return new Option<T>();
		}
		public static Option<T> Some<T>(T item){
			return new Option<T>(item, true);
		}

		public static Option<T> New<T>(T item) where T : class{
			return item == null ? None<T>() : Some<T>(item);
		}
	}

	public struct Option<T>{
		public static Option<T> None {get{
				return new Option<T>();
			}}

		public Option(T item, bool hasValue){
			this.item = item;
			this.hasValue = hasValue;
		}
		T item;
		bool hasValue;
		public bool HasValue{get {return hasValue;}}
		public T Value{
			get{
				if(!hasValue) throw new Exception("No value");
				return item;
			}
		}
	}
}

