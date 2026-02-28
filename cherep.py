from turtle import *
tracer(0)
k = 30

right(90)

for i in range(4):
    forward(k * 8)
    right(150)
    forward(k* 8)
    right(30)

penup()

for x in range(-20, 20):
    for y in range(-20, 20):
        setpos(k * x, k * y)
        dot(4, "red")
done()
        