# The Lox Interpreter, CSharp Version

This is a WIP of the Lox Interpreter, following the book Crafting Interpreters by Robert Nystrom.

## TODO
[ ] Interpreter: Error Handling: show the user the column where the syntax error occurred  
[ ] Interpreter: Error Handling: make the Run function return the error, not rely on some "global" variable  
[ ] Unescape \n, \t  
[ ] Support escaping " in strings  
[ ] Test multiline strings  
[ ] Test that the interpreter doesnt accept dot terminated numbers  
[ ] Book Challenge: Implement /**/ comments, with nesting support  
[ ] Interpret comments to allow parsing of documentation  
[ ] AST: implement 'visitor.Accept' method only on the Expr abstract class intead of on each subclass
[ ] Implement basic arithmetic interpreter  
