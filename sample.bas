var start := 0
var limit := 5
var total := 0
var counter := 0
var name := "Ada"
var enabled := true
var blocked := false

if (enabled && !blocked) || start = 0 then
    print("compiler start")
else
    print("compiler blocked")
endif

while counter < limit do
    total := total + (counter * 2)
    counter := counter + 1
endwhile

var average := total / limit
var done := counter >= limit && total != 0
var isSmall := total < 10
var isEqual := total = 10
var isNotEqual := total != 11
var isLessOrEqual := average <= 5
var isGreaterOrEqual := average >= 2

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
