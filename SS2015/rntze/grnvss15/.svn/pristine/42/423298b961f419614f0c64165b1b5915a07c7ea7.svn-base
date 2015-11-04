CC=gcc
CFLAGS=-Wall -O2
LDFLAGS=
TARGET=udpchat

SOURCES=src/udpchat.o

OBJECTS=$(SOURCES:.c=.o)


all: $(SOURCES) $(TARGET)

$(TARGET): $(OBJECTS)
	$(CC) $(LDFALGS) $(OBJECTS) -o $@

.c.o:
	$(CC) -c $(CFLAGS) $< -o $@

clean:
	rm -rf $(SOURCES)
	rm -rf $(TARGET)
