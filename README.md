# Sqript
```
      _______/ ^ \___.___,
     (__.   ._ .     |
     .__)(_][  | [_) | _.
      \    |     |     /
```
### ~ another (dynamic|static) type-(less|full) adventure into obscurity

Sqript is an interpreting programming language written in C#.
It is being developed and maintained by David Neumaier aka 'Qrakhen' (qrakhen@gmail.com, http://qrakhen.net)

It comes with its very own syntax, data types, workflow and behaviours.
Can be built for - and also runs on - all platforms, including your microwave.

https://github.com/qrakhen/sqript

## Philosophy

With Sqript, I wanted to create something that is new and different,
but still logical, consistent and not too alien.
My very first idea was mostly concerned about the looks - hence why we have ` *~ a <~ 5 `.
More and more ideas came together, and as stuff started to work, 
I created a concept that I now call ` Sqript `. 
Sqript fixes logical issues that other languages (in my opinion) suffer from,
and takes the best parts of what I personally like the most, sticking them together - 
whilst trying to make the result as consistent as possible.
It was never intended to be used as an actual thing used in real productive scenarios,
but it definitely has the power to do so.

Please keep in mind that Sqript is still very young and will suffer from child disease - 
but as I am constantly working on it, even these things will fade.

### Some Features:
 * type-less, comparable to JavaScript, but CAN be typed if wanted.
 * easily extendendable - missing a feature? implement it!
 * pointers. just in easy-to-understand and hard-to-fuck-up.
 * encapsulation (private, protected, public)
 * namespaces, classes, inheritation
 * configurable keyword alternatives
 * operator overloads, custom operator declaration and definition
 * custom 'native' types, extendable native types

# OUTDATED
## Please be patient while I am working on an actual documentation for this.

### Example Code
```javascript
 # reference declaration & assignment
 <<	*~ name <~ 'foo';
 >> foo
 << name <~ name + 'bar';
 >> foobar
 
 # funqtion declaration
 << *~ pow <~ ~(value {
        <~ value * value;
    });
 << pow(5);
 >> 25

 # assign by reference
 << *~ a <~ 5;
 >> 5
 << *~ b <& a;
 << a <~ 10;
 << b;
 >> 10
 
 # typeless per default, but can be strictly typed
 *<string>~ str = "I'm a string!";
 *<number>~ num = 12.74;
 str = num * 2; # throws Sqript.OperationException

 # fixed-type function declaration
 funqtion<string> concat(<string>str1 <string>str2 {
     <~ str1 + str2;
 });
concat(str, num); # throws Sqript.FunctionParameterViolation
```

### Primitive Types
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
#### INTEGER
```javascript
<int> [0]
```

#### DECIMAL
```javascript
<dec> [0.0]
```

#### BOOLEAN
```javascript
<bool> [true|false]
```

### Collection Types
#### Array
Integer indexed lists that store references to any value type.
Items can be accessed using the ` : ` delimiter.
```javascript
[(<any>, ...)];
*~ array <~ [3, 'str', { a <~ 'b' }];
array:0		// -> 3
array:2:a	// -> 'b'
```

### Context Types
Context types store properties indexed by keys that can be accessed via the ` : ` delimiter:
```javascript
ref ctx <~ {};
ctx:a <~ 'apple';
ctx:b <~ 5 + 3;
{
	a <~ 'apple',
	b <~ 8
}
```
Every context can always look up recursively to its parents for identifiers.

#### Obqects
Yes, I called objects obqects.
But no worries, you'll never have to actually type that.
String indexed collection that stored references to any value type
```javascript
ref obqect <~ { a <~ 5, b <~ 'c' };

*~ obj <~ {};
obj:name <~ 'Elephant';
obj:child <~ {};
obj:child:name <~ 'Noodles';
```

#### Qlasses
Yes, that's the word i chose for classes. Problem?
Don't worry, you can still use the 'class' keyword alternative.
```javascript
soon(tm);
```

#### Funqtions
And again. That's how real my naming is.
The default keyword aliases are ` function | func | fq | *:`.

It is nothing but a container that literally stores statements for later execution.

The funqtion syntax might seem uncommon:
```javascript
function multiply(a b {
	<~ a * b;
});
```
This is a simple form a funqtion with two parameters and a return statement (` <~ ` is a return alias).
Notice that there's no comma between the parameters? Yes. That's because you don't need them, ever.
You might also wonder why the entire funqtion body is wrapped in ` () `.

Here's why:
```javascript
fq multiply(a b {
	<~ a * b;
}, a b c {
	<~ a * b * c;
});

Yes, that's right: funqtion overloads work exactly like this and are all declared in one body.
Of course overloaded funqtions don't make much sense without types, so here's a (pretty pointless) typed version:
```javascript
fq multiply(<decimal>a <int>b <int>{
	<~ (int)math.floor(a * (decimal)b);
}, <int>a <string>b <int>c <string>{
	ref<string> str = "";
	step (i; 0; c) {
		str = str + (" ":pad(a) + b) + "\n";
	}
	<~ str; 
});

multiply(4, 'hello', 3); // would result in:
 ~>     hello
        hello
        hello
        hello
```

#### The Global Context
The Global Context is is a static funqtion type context that loads the initial file (or dynamically reads from a text stream like a command line).
It only has very few funqtions assigned, such as ` qonfig(); ` or ` include(); ` which can be renamed or even removed if wanted.

#### Namespace Context
Coming soon (tm)

### Keywords
#### Reference & Dereference
The reference keyword is used to declare variables that can store any value type per default
```javascript
reference | ref | declare | *~
dereference | del | destroy | ~:

// dynamic declaration
*~ name <~ 'Max';
// Max

*~ name = 123;
// Exception thrown at stdin 0:4, cannot redeclare reference 'name'

~: name;
// print(name) Exception thrown at stdin 0:4, undefined reference 'name' detected

// static typed declaration
ref<int> number = 10;

// or with the special *~ alternative
*dec~ number = 3.72;
```

### Operators
#### Assign (by Value ' <~ ' or Reference ' <& ')
```javascript
name <~ 'Der' + 'ser'; // assigns value of right hand operation result
*~ nameReference <& name; // assigns the actual variable by reference, so

name <& 'Foo'; // would result in
(nameReference == 'Foo');
```


### Qonfiguration
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
logLevel		| DEVELOPMENT	| set logging level
forceTypes		| false			| forces type usage
forceNamespace	| false			| forces namespace usage
allowRedeclare  | false			| allows reference redeclaration
allowAliases	| true			| allows keyword aliases
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
    0 = MUFFLE		//disable all output
    1 = CRITICAL	//critical errors, such as program termination
    2 = WARNINGS	//warnings such as redeclaration of references
    3 = INFO		//default logs
    4 = VERBOSE		//verbose mode, spits out everything ever
```