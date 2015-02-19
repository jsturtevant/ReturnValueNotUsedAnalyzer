public class ClassWithoutReturnResult
{
	public static void DoNotUseResult()
    {
        TestMethod();
    }

	public static string TestMethod()
    {
        return string.Empty;
    }
}