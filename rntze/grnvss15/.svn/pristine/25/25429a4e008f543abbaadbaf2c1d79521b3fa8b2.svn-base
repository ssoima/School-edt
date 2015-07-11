TARGET=asciiclient
CFLAGS+=-Wall -Wextra -g -pedantic -fdiagnostics-color=always -fno-strict-aliasing
OBJECTS=src/$(TARGET).o src/assignment4.o

export LANG = en_US.utf8

.PHONY: all clean fresh

all: $(TARGET)

$(TARGET): $(OBJECTS)
	$(CC) $(OBJECTS) $(LDFLAGS) -o $(TARGET)

clean:
	-rm -f $(OBJECTS)
	-rm -f $(TARGET)

fresh: clean all
