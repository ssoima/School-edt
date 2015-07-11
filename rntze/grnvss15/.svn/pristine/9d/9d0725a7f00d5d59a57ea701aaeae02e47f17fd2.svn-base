TARGET=libraw.a
LDFLAGS=
CFLAGS+=-Iinclude -Wall -Wextra -pedantic -O2 -fdiagnostics-color=always -fPIC

OBJECTS=src/raw.o src/hexdump.o

.PHONY: all clean fresh

all: $(TARGET)

$(TARGET): $(OBJECTS)
	ar rcs $(TARGET) $(OBJECTS)

clean:
	-rm -rf $(OBJECTS)
	-rm -f $(TARGET)

fresh: clean all
