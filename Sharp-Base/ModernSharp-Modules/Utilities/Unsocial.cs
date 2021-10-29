using System;

namespace ModernSharp_Modules.Utilities {
    /// <summary>Wrapper for Enums flags to restrict use to only a single flag.</summary>
    /// <typeparam name="T">Enum type attached to this Unsocial.</typeparam>
    public class Unsocial<T> where T : struct, IComparable {
        #region Private Fields
        /// <summary>Gets the flag set for this object.</summary>
        private T Flag { get; set; }
        #endregion

        #region Constructors
        /// <summary>Constructs an Unsocial with the provided flag of type T.</summary>
        /// <param name="flag">Flag to set.</param>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="ArgumentException"/>
        public Unsocial(T flag) {
            if(!typeof(T).IsEnum)
                throw new NotSupportedException();

            bool isDefinedFlagNotSet = !Enum.Equals(Flag, flag) && Enum.IsDefined(typeof(T), flag);

            if(isDefinedFlagNotSet) {
                Flag = flag;
            } else {
                throw new ArgumentException("Cannot set non-existant or multiple flag(s) in Unsocial for enum: " + typeof(T).ToString());
            }
        }
        #endregion

        #region Parsing Interface
        /// <summary>Parses a member name into a member of type T if possible, otherwise null is returned.</summary>
        /// <param name="name">Name of the member to parse.</param>
        /// <returns>A nullable value of type T of the member that was parsed.</returns>
        public static Unsocial<T> Parse(string name) {
            Unsocial<T> result = null;
            T value;

            if(Enum.TryParse<T>(name, out value))
                result = new Unsocial<T>(value);

            return result;
        }

        /// <summary>Creates a new Unsocial with the specified flag.</summary>
        /// <param name="flag">Enum flag to set.</param>
        /// <returns>New Unsocial.</returns>
        public static Unsocial<T> Get(T flag) {
            return new Unsocial<T>(flag);
        }
        #endregion

        #region Get Generic Type
        /// <summary>Gets the type of T of this object.</summary>
        /// <returns>The underlying type of the enum of T of this generic object.</returns>
        public Type GetGenericType() =>
            typeof(T);
        #endregion

        #region Comparions / Equals Interface
        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns> A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes obj in the sort order. Zero This instance occurs in the same position in the sort order as obj. Greater than zero This instance follows obj in the sort order.</returns>
        public int CompareTo(Unsocial<T> obj) =>
            Flag.CompareTo(obj.Flag);

        /// <summary>Gets whether two objects equal (they share the same reference).</summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>True if the provided object is equal to this object.</returns>
        public bool Equals(Unsocial<T> obj) {
            if(obj.GetType() != GetType())
                return false;
            return this.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object, else false.</returns>
        public override bool Equals(object obj) =>
            Equals((Unsocial<T>)obj);

        /// <summary>Gest the hash code for the current object.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() =>
            Flag.GetHashCode() ^ typeof(T).GetHashCode();
        #endregion

        #region Operator Overloads
        /// <summary>Gets whether two objects contain the same flag.</summary>
        /// <param name="obj1">First object to compare.</param>
        /// <param name="obj2">Second object to compare.</param>
        /// <returns>True if the flag of each object is the same, else false.</returns>
        public static bool operator ==(Unsocial<T> obj1, Unsocial<T> obj2) =>
            obj1.Equals(obj2);

        /// <summary>Gets whether two objects do NOT contain the same flag.</summary>
        /// <param name="obj1">First object to compare.</param>
        /// <param name="obj2">Second object to compare.</param>
        /// <returns>True if the flag of each object is NOT the same, else false.</returns>
        public static bool operator !=(Unsocial<T> obj1, Unsocial<T> obj2) =>
            !obj1.Equals(obj2);

        /// <summary>Gets the underlying flag.</summary>
        /// <param name="obj">Unsocial to pull the flag from.</param>
        public static implicit operator T(Unsocial<T> obj) =>
            obj.Flag;

        /// <summary>Gets a Unsocial from a flag.</summary>
        /// <param name="obj">Flag to pull the Unsocial from.</param>
        public static implicit operator Unsocial<T>(T obj) =>
            new Unsocial<T>(obj);
        #endregion
    }
}
