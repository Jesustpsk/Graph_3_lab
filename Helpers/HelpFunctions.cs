namespace Graph_3_lab.Helpers;

public static class HelpFunctions
{
    public static bool IsEmptyOrWhiteSpace(this string input)
    {
        return input == "" || input == " ";
    }
}