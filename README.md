# sqript
### ~ another (dynamic|static) type-(less|full) adventure into obscurity

Sqript is a interpreted programming language written in C#.

It comes with it's very own syntax, data types, workflow and behaviours.
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
```javascript
<string> ['abc', "abc"]
<str>
```
Strings, yea. Can be concatinated using a + b;
#### NUMBER
```javascript
<num> [0.0, 0]
```
Dynamic number type, can represent an integer or a decimal number.
##### INTEGER
```javascript
<int> [0]
```
##### DECIMAL
```javascript
<dec> [0.0]
```
#### BOOLEAN
```javascript
<bool> [true|false]
```
#### COLLECTION
```javascript
<collection> [*]
```
Abstract type, inherited by Array and Obqect.
##### Array
```javascript
<array> [index:*]
<arr>
```
##### Obqect
```javascript
<object> [key:*]
<obq> 
```
#### NULL
Yes that's a type.
No it can not be declared.

### Keywords
#### Reference
```javascript
reference | ref | declare | *~

// dynamic declaration
*~ name = 'Max';

// static declaration
ref<int> number = 10;

// or with the special *~ alternative
*dec~ number = 3.72;
```

### Qlasses
Yes, that's the word i chose for classes. Problem?
Don't worry, you can still use the 'class' keyword alternative :)


### Funqtions
And again. That's how real this is.
Yes, you can also use the keywords "function", "fn", "fq" or even "()~>" here.
Don't ask how I came up with `()~>`.

### Obqects
:)

### Operators
#### Assign (by Value or Reference)
```javascript
name = 'Der' + 'ser'; // assigns value of right hand operation result
*~ nameReference <& name; // assigns the actual variable by reference, so

name = 'Foo'; // would result in
(nameReference == 'Foo');
```

### Syntax
#### Obqect Syntax
```javascript
// :  -> key delimiter
// {} -> object declaration

*~ obj = {};
obj:name = 'Elephant';
obj:child = {};
obj:child:name = 'Noodles';
```

### Configuration
```javascript
:qonfig(configKey, value);

:qonfig('forceTypes', true); // force static reference type declaration
:qonfig('forceTypes');	// resets to default value (from qonfig.json)
						// auto-reset when a new file is being read
```

#### Program/Project wide Configuration
```javascript
$ vi /path/to/your_sqrapp/qonfig.json
```
When no qonfig file is provided, default values will be used.

#### Configuration Options
```javascript
key				| default		| values
----------------|---------------|---------------------------------------
logLevel		| DEVELOPMENT	| read below (sets console logging level)
forceTypes		| false			| false, true
forceNamespace	| false			| false, true
timeout		    | 60			| [any integer] (in seconds)
allowInput		| true			| false, true (enables stdIn)
entryPointFile  | main.sqr		| [any string] (path to entry point file)
callStackLimit  | 128			| [any integer] (per function or constructor call)
ignoreLines		| false			| false, true (ignores faulty lines)
runtimeCore		| sqript.core	| [any string] (see runtime cores)
useProcessOut	| true			| false, true (use stdOut internally)
encoding		| UTF-8			| [any string] (encoding type)
suppressErrors  | false			| false, true (does not throw exceptions)
```

##### logLevel
Can be set to one of these values:
```javascript
    MUFFLE		//disable all output
    CRITICAL	//critical errors, such as program termination
    WARNINGS	//warnings such as redeclaration of references
    INFO		//default logs
    VERBOSE		//verbose mode, spits out everything ever
```