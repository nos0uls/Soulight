from turtle import *
tracer(0)
k = 10


for i in range(2):
   forward(k * 6)
   right(90)
   forward(k *12 )
   right(90)
penup()

backward(k * -2)
right(90)
forward(k * 9)
left(90)

pendown()
for i in range(4):
    forward(8 * k)
    right(90)

   

penup()
for x in range(-20, 20):
    for y in range(-20, 20):
        setpos(x * k, y * k)
        dot(4, "red")


done()
