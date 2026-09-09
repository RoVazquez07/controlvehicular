import { useState, type ReactNode } from 'react'

type ICheckProps = {
  checked: boolean
  onChange: (checked: boolean) => void
  children?: ReactNode
  required?: boolean
  name?: string
}

const helperStyle = {
  position: 'absolute',
  top: '0%',
  left: '0%',
  display: 'block',
  width: '100%',
  height: '100%',
  margin: 0,
  padding: 0,
  background: '#fff',
  border: 0,
  opacity: 0,
} as const

export function ICheck({
  checked,
  onChange,
  children,
  required,
  name,
}: ICheckProps) {
  const [hover, setHover] = useState(false)

  const boxClass = [
    'icheckbox_square-green',
    hover ? 'hover' : '',
    checked ? 'checked' : '',
  ]
    .filter(Boolean)
    .join(' ')

  return (
    <div className="checkbox i-checks">
      <label
        onMouseEnter={() => setHover(true)}
        onMouseLeave={() => setHover(false)}
      >
        {' '}
        <div className={boxClass} style={{ position: 'relative' }}>
          <input
            type="checkbox"
            name={name}
            checked={checked}
            required={required}
            onChange={(event) => onChange(event.target.checked)}
            style={{ position: 'absolute', opacity: 0 }}
          />
          <ins className="iCheck-helper" style={helperStyle} />
        </div>
        {children}
      </label>
    </div>
  )
}
