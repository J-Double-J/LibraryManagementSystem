using Domain.Abstract;

namespace UnitTests
{
    // Gives tests an easy place to reference some pre-constructed objects for tests.
    public static class TestReferenceHelper
    {
        public static ErrorCode TEST_CODE = new("UnitTestError", "Testing", "AnExpectedError");
        public static Error TEST_ERROR = new(TEST_CODE, "This was designed to be thrown in the test");
    }
}
