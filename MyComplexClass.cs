using System;

namespace NUnitSnapshotExample
{
    public class MyComplexClass
    {
        private readonly ComplexSubClass _subField1;
        public String Field1 { get; }
        public String Field2 { get; }
        public String SubField1 => _subField1.Field1;
        public String Field3 { get; }

        public MyComplexClass()
        {
            Field1 = "something";
            Field2 = "somethingElse";
            Field3 = "anotherSomething";
            _subField1 = new ComplexSubClass();
        }

        private class ComplexSubClass
        {
            public String Field1 { get; }

            public ComplexSubClass()
            {
                Field1 = "another thing";
            }
        }
    }
}