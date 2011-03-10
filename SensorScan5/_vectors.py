from visual import *

def pause(sec):
    for i in range(1,sec):
        rate(50)
        
v0=vector(0,0,0)
v1=vector(-.8,-.8,-.58)
v2=vector(-.42,-.32,.12)

f = frame()

a1 = arrow(frame=f, pos=v0, axis=v1, shaftwidth=0.1, color=(.8,.8,0))
a2 = arrow(frame=f, pos=v0, axis=v2, shaftwidth=0.1, color=(0,.8,.8))

mybox = box(frame=f, pos=(0,0,0), length=.3, height=.1, width=.6)
mybox2 = box(frame=f, pos=(0,.05,0), length=.3, height=.01, width=.6, color=(.8,.8,.8))
mybox2 = box(frame=f, pos=(0,.05,0.055), length=.2, height=.01, width=.3, color=(1,1,1))

#
x = arrow(frame=f, pos=(-1,0,0), axis=(2,0,0), shaftwidth=0.0005, color=(1,0,0))
y = arrow(frame=f, pos=(0,-1,0), axis=(0,2,0), shaftwidth=0.0005, color=(0,1,0))
z = arrow(frame=f, pos=(0,0,-1), axis=(0,0,2), shaftwidth=0.0005, color=(0,0,1))

tx = text (frame=f, pos=(1.1,0,0), text='x', height=.2, depth=.01)
ty = text (frame=f, pos=(0,1.1,0), text='y', height=.2, depth=.01)
tz = text (frame=f, pos=(0,0,1.1), text='z', height=.2, depth=.01)
#t1 = text (frame=f, pos=(0,1,1.1), text='Hallo', height=.2, depth=.01)

#rotate y plane
#f.rotate(angle=radians(12.5),axis=(0,0,-1), pos=(0,0,1))
pause(100)
#rotate x plane
f.rotate(angle=radians(22.5),axis=(0,-1,0), pos=(0,1,0))
pause(100)
#rotate z plane
f.rotate(angle=radians(-22.5),axis=(-1,0,0), pos=(1,0,0))

# a1.color = color(1,0,0)

