# sqript
### ~ another type-less adventure into obscurity

Sqript is a programming language interpreter written in C#
It serves comes with it's very own syntax, data types, workflow and behaviours.
Can be built for - and also runs on - all platforms, including your microwave.

### Some Features:
 * type-less, comparable to JavaScript, but CAN be typed if wanted.
 * encapsulation (private, protected, public)
 * namespaces, classes, inheritation
 * C++ like pointers, just in easy-to-understand and hard-to-fuck-up.
 * configurable keyword alternatives
 * custom 'native' types, extendable native types

### Example Code
```javascript
// reference declaration & assignment
reference name;
name = 'Foo';
name = name + 'Bar';

// several (configurable) keyword alternatives:
ref a = 7;
*~ b = a + 5; // yes, *~ is a valid 'ref' alternative

// actual forced references, even to primitive types:
ref a = 5;
ref b <& a; // <& means force reference assignment, no matter what the type is
a = 10;		// b is now also 10
b = 7;		// a is now also 7
a <& 2;		// new integer assigned by reference, so
b = 5;		// and a stays 2.

// typeless per default, but can be strictly typed
ref<STRING> str = "I'm a string!";
ref<NUMBER> num = 12.74;
str = num * 2; // throws Sqript.OperationException

// typeless function
function concat(str1, str2) {
	<~ str1 + str2; // <~ is a return keyword
}
concat(str, num); // would return "I'm a string!12.72";

// fixed type function
function<STRING> concat(STRING:str1, STRING:str2) {
	<~ str1 + str2;
}
concat(str, num); // throws Sqript.FunctionParameterViolation

// classes
// not yet implemented

// to be continued, lots more to be written
```

### Types
#### STRING
Strings, yea. Can be concatinated using a + b;
#### NUMBER
Dynamic number type, can represent an integer or a decimal number.
##### INTEGER
##### DECIMAL
#### BOOLEAN
#### COLLECTION
Abstract type, inherited by Array and Obqect.
##### Array
##### Obqect
#### NULL
Yes that's a type.