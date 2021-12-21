namespace CodeFileTools
{
    public class RedisValue 
    {
        public RedisValue(string val)
        {
            this.val = val;
        }
        public string val { get; set; }

        public static implicit operator RedisValue(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return string.Empty;
            
            return new RedisValue(value);
        }

        public static implicit operator string(RedisValue value)
        {
            return value.val;
        }
    }
}
