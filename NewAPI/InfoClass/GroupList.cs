namespace NewAPI.InfoClass
{
    class GroupList
    {
        public string GroupID { get; set; }
        public bool photo { get; set; }
        public bool text { get; set; }
        public bool poll { get; set; }
        public bool audio { get; set; }
        public bool video { get; set; }
        public bool BreakMedia { get; set; }
        public bool AttachmentsNullBlock { get; set; }
        public bool ModerationNews { get; set; }
        public int LastTime { get; set; }
    }
}
