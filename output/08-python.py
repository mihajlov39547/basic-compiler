start = 0
limit = 5
total = 0
counter = 0
name = "Ada"
enabled = True
blocked = False
if enabled and not (blocked) or start = 0:
    print("compiler start")
else:
    print("compiler blocked")
while counter < limit:
    total = total + counter * 2
    counter = counter + 1
average = total / limit
done = counter >= limit and total != 0
isSmall = total < 10
isEqual = total = 10
isNotEqual = total != 11
isLessOrEqual = average <= 5
isGreaterOrEqual = average >= 2
print(name)
print("total=")
print(total)
print("average=")
print(average)
print("done?")
print(done)
print("small?")
print(isSmall)
print("equal?")
print(isEqual)
print("not equal?")
print(isNotEqual)
print("leq?")
print(isLessOrEqual)
print("geq?")
print(isGreaterOrEqual)
