namespace Excalibur
{
    public static class RegisterdEvent
    {
        /// <summary>
        /// 事件处理器的时间名，保持开头大写
        /// </summary>
        public static readonly string
            /// Excample
            Excample = "",
            /// Template，便于新增事件名时不修改逗号分号，新增的事件名加在这个前面，以逗号结尾
            Template = "";

        static RegisterdEvent ()
        {
            System.Reflection.FieldInfo[] fields = typeof (RegisterdEvent).GetFields ();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue (field.Name, string.Format ("Event_{0}", field.Name));
            }
        }
    }
}