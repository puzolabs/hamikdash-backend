
CREATE TABLE IF NOT EXISTS altars
(
    id serial,
    is_available boolean DEFAULT TRUE NOT NULL,
    CONSTRAINT altar_pkey PRIMARY KEY (id)
)