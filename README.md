# UnityDisposePatternExtension
Initial Commit

Aspect Oriented programming is one of those paradigms that allows us to separate cross cutting concerns. 
One of the methods that unity provides us with, is the interceptor pattern.

In this repository, I show a "weaving"/composition pattern using a custom unity builder strategy using a common implementation of IDisposable.

What we'll do is declare a simple class that implements a simple interface, and then tell the container to weave in the implementation of IDisposable when resolving the object.

The resolved object will then implement IDisposable, and we'll be able to use the debugger to drop down to our weaved implementation.

