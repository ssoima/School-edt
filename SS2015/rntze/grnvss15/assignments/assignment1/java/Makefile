TARGET=wol.jar

LIBSOCKET=deps/libGRNVS.so
LIBRAWDIR=deps/libraw/
LIBRAW=$(LIBRAWDIR)libraw.a
LDFLAGS=$(LIBRAW)
CFLAGS+=-I$(LIBRAWDIR)include -I/usr/lib/jvm/java-8-openjdk-amd64/include
CFLAGS+=-I/usr/lib/jvm/java-8-openjdk-amd64/include/linux -Ideps -fPIC
CFLAGS+=-fdiagnostics-color=always


CLASSES=Arguments.class GRNVS_RAW.class Assignment1.class Timeout.class
LFILES=$(addprefix -C src , $(CLASSES))
OBJECTS=$(addprefix src/, $(CLASSES))
SOURCES=$(OBJECTS:.class=.java)

LIBSOCKETOBJECTS= deps/GRNVS_RAW.o

all: $(TARGET) $(LIBSOCKET) wol

.PHONY: all clean fresh

$(TARGET): $(OBJECTS)
	jar cfm $(TARGET) deps/manifest.txt $(LFILES)

%.class: %.java
	javac $(SOURCES)

wol:
	cp deps/run wol

$(LIBSOCKET): $(LIBSOCKETOBJECTS) $(LIBRAW)
	$(CC) $(LIBSOCKETOBJECTS) $(LDFLAGS) -shared -o $(LIBSOCKET)

$(LIBRAW):
	cd $(LIBRAWDIR); make

clean:
	-rm -f $(OBJECTS)
	-rm -f $(TARGET)
	-rm -f $(LIBSOCKETOBJECTS)
	-rm -f $(LIBSOCKET)
	-rm -f wol

fresh: clean all
