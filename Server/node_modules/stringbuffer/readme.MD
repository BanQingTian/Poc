
## Installation
```
npm install stringbuffer
```

## Usage
```javascript
var StringBuffer = require("stringbuffer");

// create a string buffer that simply concatenates strings
sb = new StringBuffer();
sb.append("HELLO").append("WORLD");

// outputs: HELLOWORLD
console.log(sb.toString());


// you can set the separator when calling the constructor or the toString() method
sb = new StringBuffer("--");
sb.append("HELLO").append("WORLD");

// outputs: HELLO--WORLD
console.log(sb.toString());

// outputs: HELLO**WORLD
console.log(sb.toString("**"));


// use the NEWLINE constant to create different lines
sb = new StringBuffer();
sb.append("LINE1").append("LINE2");

// outputs: LINE1\r\nLINE2 in Windows or LINE1\nLINE2 otherwise
console.log(sb.toString());


// concatenate multiple buffers
header = new StringBuffer();
content = new StringBuffer();
footer = new StringBuffer();

header.append("<HEADER>");
content.append("LINE1").append("LINE2").append("LINE3");
footer.append("<FOOTER>");

// outputs: <HEADER>LINE1LINE2LINE3<FOOTER>
console.log(header.append(content).append(footer).toString());

```
