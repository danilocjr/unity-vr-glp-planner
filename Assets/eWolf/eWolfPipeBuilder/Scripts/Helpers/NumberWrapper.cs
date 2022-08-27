namespace eWolf.PipeBuilder.Helpers
{
    public abstract class NumberWrapper
    {
        /// <summary>
        /// The value - wrapped around the _min and _max
        /// </summary>
        private float _value;

        /// <summary>
        /// The minimal value
        /// </summary>
        private readonly float _min = 0;

        /// <summary>
        /// The maximum value
        /// </summary>
        private readonly float _max = 10;

        /// <summary>
        /// Constructor that set the min and max value
        /// </summary>
        /// <param name="min">The minimal value</param>
        /// <param name="max">The maxium valus</param>
        public NumberWrapper(float min, float max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Gets or Sets the internal Get/Sets the value and wrapped it
        /// </summary>
        protected float ValueImp
        {
            get { return _value; }
            set
            {
                _value = value;

                while (_value > _max)
                    _value -= _max;

                while (_value < _min)
                    _value += _max;
            }
        }
    }
}