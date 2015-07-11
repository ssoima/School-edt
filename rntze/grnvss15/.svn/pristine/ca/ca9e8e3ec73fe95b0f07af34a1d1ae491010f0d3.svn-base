CC=gcc
CFLAGS=-Wall -O2
LDFLAGS=
TARGET=tcpchat_server

SOURCES=src/server.c

OBJECTS=$(SOURCES:.c=.o)


all: $(SOURCES) $(TARGET)

$(TARGET): $(OBJECTS)
	$(CC) $(LDFALGS) $(OBJECTS) -o $@

.c.o:
	$(CC) -c $(CFLAGS) $< -o $@

clean:
	rm -fv $(TARGET)
	rm -fv $(OBJECTS)
