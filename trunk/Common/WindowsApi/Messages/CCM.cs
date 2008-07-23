namespace Common.WindowsApi.Messages {
    public enum CCM : int {
        FIRST = 0x2000,// Common control shared messages
        SETUNICODEFORMAT = (FIRST + 5),
        GETUNICODEFORMAT = (FIRST + 6)
    }
}