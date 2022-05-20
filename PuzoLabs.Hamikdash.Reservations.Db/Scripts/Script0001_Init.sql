
CREATE TABLE IF NOT EXISTS altars
(
    id serial,
    is_available boolean DEFAULT TRUE NOT NULL,
    CONSTRAINT altar_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

CREATE TABLE IF NOT EXISTS reservations
(
    id uuid,
    type text,
    altarId integer,
    startDate timestamp,
    endDate timestamp,
    userId uuid,
    status text,
    CONSTRAINT reservation_pkey PRIMARY KEY (id)
)
