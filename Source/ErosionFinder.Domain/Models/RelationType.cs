namespace ErosionFinder.Domain.Models
{
    public enum RelationType
    {
        /// <summary>
        /// Some components inherits from another component
        /// </summary>
        Inheritance = 1,

        /// <summary>
        /// Some type is returned by a function
        /// </summary>
        ReturnByFunction = 2,

        /// <summary>
        /// Some type is received as parameter in a function
        /// </summary>
        ReceiptByMethodArgument = 3,

        /// <summary>
        /// Some type is received as parameter in a constructor
        /// </summary>
        ReceiptByConstructorArgument = 4,

        /// <summary>
        /// Some type derived from Exception is thrown
        /// </summary>
        Throw = 5,

        /// <summary>
        /// Some type is instantiated
        /// </summary>
        Instantiate = 6,

        /// <summary>
        /// Some type is declared
        /// </summary>
        Declarate = 7,

        /// <summary>
        /// Some type's function is invoked
        /// </summary>
        Invocate = 8,

        /// <summary>
        /// Some type is indirectly referenced; 
        /// ex: SomeClass : AnotherClass<IndirectClass>
        /// </summary>
        Indirect = 9
    }
}