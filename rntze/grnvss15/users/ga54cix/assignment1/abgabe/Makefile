TARGET=wol
LIBRAWDIR=libraw/
LIBRAW=$(LIBRAWDIR)libraw.a
LDFLAGS=$(LIBRAW)
CFLAGS+=-Wall -Wextra -fdiagnostics-color=always -I$(LIBRAWDIR)include

OBJECTS=src/wol.o src/assignment1.o

.PHONY: all clean fresh

all: $(TARGET)

$(TARGET): $(OBJECTS) $(LIBRAW)
	$(CC) $(OBJECTS) $(LDFLAGS) -o $(TARGET)

$(LIBRAW):
	cd $(LIBRAWDIR); make

clean:
	-rm -rf $(OBJECTS)
	-rm -f $(TARGET)

fresh: clean all
