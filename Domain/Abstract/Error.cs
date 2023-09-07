namespace Domain.Abstract
{
    public class Error : IEquatable<Error>
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; private init; }

        public string Message { get; private init; }

        public static implicit operator string(Error error) => error.Code;

        public static bool operator ==(Error? left, Error? right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if(left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Error? left, Error? right)
        {
            return !(left == right);
        }

        public bool Equals(Error? other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            return other.Code == Code;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (obj is not Error error)
            {
                return false;
            }

            return error.Code == Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
