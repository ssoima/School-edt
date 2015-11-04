TARGET=ndisc
LIBRAWDIR=libraw/
LIBRAW=$(LIBRAWDIR)libraw.a
LDFLAGS=$(LIBRAW)
CFLAGS+=-Wall -Wextra -g -pedantic -fdiagnostics-color=always -I$(LIBRAWDIR)include -fno-strict-aliasing

OBJECTS=src/ndisc.o src/assignment2.o

.PHONY: all clean fresh

all: $(TARGET)

$(TARGET): $(OBJECTS) $(LIBRAW)
	$(CC) $(OBJECTS) $(LDFLAGS) -o $(TARGET)

$(LIBRAW):
	cd $(LIBRAWDIR) && make

clean:
	-rm -rf $(OBJECTS)
	-rm -f $(TARGET)

fresh: clean all
